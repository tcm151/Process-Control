using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using ProcessControl.Tools;
using ProcessControl.Procedural;
using Debug = UnityEngine.Debug;


namespace ProcessControl.Pathfinding
{
    public static class AStar
    {
        public static List<Vector3> FindPath(Cell[,] cells, Vector3 start, Vector3 end)
        {
            var timer = new Stopwatch();
            timer.Start();
            
            foreach (var cell in cells) cell.pathInfo.Reset();


            var startCell = ProceduralGrid.GetCellPosition(start);
            var endCell = ProceduralGrid.GetCellPosition(end);

            if (startCell is null || endCell is null)
            {
                Debug.Log("START OR END PATHING ERROR!");
                Debug.Log(timer.ElapsedMilliseconds);
                timer.Stop();
                return null;
            }

            if (!endCell.buildable)
            {
                Debug.Log("END POINT WAS NOT NAVIGABLE");
                Debug.Log(timer.ElapsedMilliseconds);
                timer.Stop();
                return null;
            }

            Debug.Log($"Start: {startCell.coords} End: {endCell.coords}");
            
            var openList = new List<Cell> { startCell };
            var closedList = new List<Cell>();
            
            startCell.pathInfo.gCost = 0;
            startCell.pathInfo.hCost = Distance(startCell, endCell);

            while (openList.Count > 0)
            {
                var currentCell = openList.OrderBy(pc => pc.pathInfo.fCost).FirstOrDefault();
                if (currentCell is null)
                {
                    Debug.Log("LIST IS EMPTY MAYBE");
                    Debug.Log(timer.ElapsedMilliseconds);
                    timer.Stop();
                    return null;
                }

                if (!currentCell.buildable)
                {
                    openList.Remove(currentCell);
                    closedList.Add(currentCell);
                    // Debug.Log("SKIP!");
                    continue;
                }

                if (currentCell == endCell)
                {
                    timer.Stop();
                    // Debug.Log($"{timer.ElapsedMilliseconds} ms");
                    var finalPath = CalculateFinalPath(endCell);
                    if (finalPath is null) Debug.Log("PATH NOT FOUND");
                    else Debug.Log($"{finalPath.Count} m path in {timer.ElapsedMilliseconds} ms");
                    return finalPath;
                }

                openList.Remove(currentCell);
                closedList.Add(currentCell);

                foreach (var neighbourCell in currentCell.neighbours)
                {
                    if (neighbourCell is null || closedList.Contains(neighbourCell)) continue;

                    int gCost = currentCell.pathInfo.gCost + Distance(currentCell, neighbourCell);
                    if (gCost < neighbourCell.pathInfo.gCost)
                    {
                        neighbourCell.pathInfo.previousInPath = currentCell;
                        neighbourCell.pathInfo.gCost = gCost;
                        neighbourCell.pathInfo.hCost = Distance(neighbourCell, endCell);
                        
                        if (!openList.Contains(neighbourCell)) openList.Add(neighbourCell);
                    }
                }
                
            }
            
            // NO OPEN NODES!
            Debug.Log("NO PATH COULD NOT BE FOUND!");
            Debug.Log(timer.ElapsedMilliseconds);
            timer.Stop();
            return null;
        }

        private static List<Vector3> CalculateFinalPath(Cell endCell)
        {
            var currentCell = endCell;
            var reversedPath = new List<Cell> { currentCell };

            while (currentCell.pathInfo.previousInPath is { })
            {
                reversedPath.Add(currentCell.pathInfo.previousInPath);
                currentCell = currentCell.pathInfo.previousInPath;
            }

            reversedPath.Reverse();
            reversedPath.RemoveAt(0);
            return reversedPath.ConvertAll(c => c.position);
        }

        private static int Distance(Cell first, Cell second)
        {
            var vector = (second.position - first.position).Abs().CeilToInt();
            return 14 * Mathf.Min(vector.x, vector.y) + 10 * Mathf.Abs(vector.x - vector.y);
        }
    }
}