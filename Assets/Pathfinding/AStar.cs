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
        public static List<Vector3> FindPath(Vector3 start, Vector3 end)
        {
            var timer = new Stopwatch();
            timer.Start();
            
            var startCell = TileGrid.GetCellAtPosition(start);
            var endCell = TileGrid.GetCellAtPosition(end);
            if (startCell is null || endCell is null)
            {
                Debug.Log($"Start or End cell(s) did not exist...");
                return new List<Vector3> {start};
            }
            if (!endCell.buildable)
            {
                Debug.Log("End tile was not navigable");
                return new List<Vector3> {start};
            }
            
            endCell.pathInfo.Reset();
            startCell.pathInfo.Reset();
            // Debug.Log($"Start: {startCell.coords} End: {endCell.coords}");
            
            var openList = new List<Cell> {startCell};
            var closedList = new List<Cell>();

            var steps = 0;
            startCell.pathInfo.gCost = 0;
            var minimumDistance = startCell.pathInfo.hCost = DistanceBetween(startCell, endCell);

            while (openList.Count > 0 && steps < minimumDistance * 4)
            {
                var currentCell = openList.OrderBy(pc => pc.pathInfo.fCost).First();
                if (!currentCell.buildable)
                {
                    openList.Remove(currentCell);
                    closedList.Add(currentCell);
                    currentCell.pathInfo.Reset();
                    continue;
                }

                if (currentCell == endCell)
                {
                    var finalPath = RetracePath(endCell);
                    if (finalPath is null)
                    {
                        Debug.Log($"Path did not exist between {startCell.coords} and {endCell.coords}");
                        return new List<Vector3> { start };
                    }
                    
                    Debug.Log($"{finalPath.Count} m path in {timer.ElapsedMilliseconds} ms");
                    return finalPath;
                }

                closedList.Add(currentCell);
                openList.Remove(currentCell);

                // add neighbours to open list
                currentCell.neighbours.ForEach(neighbourCell =>
                {
                    if (neighbourCell is null || closedList.Contains(neighbourCell)) return;

                    neighbourCell.pathInfo.Reset();
                    int gCost = currentCell.pathInfo.gCost + DistanceBetween(currentCell, neighbourCell);
                    if (gCost >= neighbourCell.pathInfo.gCost) return;
                    neighbourCell.pathInfo.previousInPath = currentCell;
                    neighbourCell.pathInfo.gCost = gCost;
                    neighbourCell.pathInfo.hCost = DistanceBetween(neighbourCell, endCell);
                    if (!openList.Contains(neighbourCell)) openList.Add(neighbourCell);
                });
            }
            
            // no open nodes left
            Debug.Log($"Open list emptied after {timer.ElapsedMilliseconds} ms");
            return new List<Vector3> { start };
        }

        private static List<Vector3> RetracePath(Cell endCell)
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

        private static int DistanceBetween(Cell first, Cell second)
        {
            var vector = (second.position - first.position).Abs().CeilToInt();
            return 14 * Mathf.Min(vector.x, vector.y) + 10 * Mathf.Abs(vector.x - vector.y);
        }
    }
}