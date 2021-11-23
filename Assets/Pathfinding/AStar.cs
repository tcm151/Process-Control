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
        //> FIND THE SHORTEST PATH BETWEEN START AND END POSITIONS
        public static List<Vector3> FindPath(Vector3 start, Vector3 end)
        {
            var timer = new Stopwatch();
            timer.Start();
            
            var startCell = CellGrid.GetCellAtPosition(start);
            var endCell = CellGrid.GetCellAtPosition(end);
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
            if (!endCell.walkable)
            {
                // Debug.Log("Destination was not walkable, using next closest walkable cell");
                endCell = endCell.neighbours.ToList().OrderBy(n => DistanceBetween(startCell, n)).First();
            }
            if (startCell == endCell)
            {
                return new List<Vector3> {end};
            }
            
            endCell.pathInfo.Reset();
            startCell.pathInfo.Reset();
            
            var openList = new List<Cell> {startCell};
            var closedList = new List<Cell>();

            startCell.pathInfo.Set(0, DistanceBetween(startCell, endCell), null);


            //- loop until path is found or limit is reached
            var steps = 0;
            var minimumDistance = DistanceBetween(startCell, endCell);
            while (openList.Count > 0 && ++steps < (minimumDistance.CeilToInt() * 32))
            {
                var currentCell = openList.OrderBy(pc => pc.pathInfo.fCost).First();
                
                // check is reached destination
                if (currentCell == endCell)
                {
                    var finalPath = RetracePath(endCell);
                    if (finalPath is {}) return finalPath;
                    
                    Debug.Log($"Path did not exist between {startCell.coords} and {endCell.coords}");
                    return new List<Vector3> {start};
                }
                
                // move cell to closed list if not walkable and continue
                if (!currentCell.walkable)
                {
                    openList.Remove(currentCell);
                    closedList.Add(currentCell);
                    currentCell.pathInfo.Reset();
                    continue;
                }

                // move cell to closed list
                closedList.Add(currentCell);
                openList.Remove(currentCell);

                //- add neighbours to open list
                for (int i = 0; i < currentCell.neighbours.Length; i++)
                {
                    var neighbourCell = currentCell.neighbours[i];
                    if (neighbourCell is null) continue;
                    
                    if (closedList.Contains(neighbourCell) || openList.Contains(neighbourCell)) continue;
                    if (!neighbourCell.buildable || !neighbourCell.walkable)
                    {
                        closedList.Add(neighbourCell);
                        neighbourCell.pathInfo.Reset();
                        continue;
                    }
                    
                    bool skipped = false;
                    switch (i)
                    {
                        case 0: if (!neighbourCell.neighbours[6].walkable || !neighbourCell.neighbours[4].walkable) skipped = true; break;
                        case 2: if (!neighbourCell.neighbours[3].walkable || !neighbourCell.neighbours[6].walkable) skipped = true; break;
                        case 5: if (!neighbourCell.neighbours[1].walkable || !neighbourCell.neighbours[4].walkable) skipped = true; break;
                        case 7: if (!neighbourCell.neighbours[1].walkable || !neighbourCell.neighbours[3].walkable) skipped = true; break;
                    }

                    if (!skipped && !openList.Contains(neighbourCell))
                    {
                        neighbourCell.pathInfo.Reset();
                        float gCost = currentCell.pathInfo.gCost + DistanceBetween(currentCell, neighbourCell);
                        if (gCost >= neighbourCell.pathInfo.gCost) continue;
                        neighbourCell.pathInfo.Set(gCost, DistanceBetween(neighbourCell, endCell), currentCell);
                        
                        openList.Add(neighbourCell);
                    }
                }
            }
            
            //- no open nodes left
            Debug.Log($"Open list emptied after {timer.ElapsedMilliseconds} ms");
            return new List<Vector3> { start };
        }
        
        //> RETRACE THE AND RETURN SHORTEST PATH
        private static List<Vector3> RetracePath(Cell endCell)
        {
            var currentCell = endCell;
            var reversedPath = new List<Cell> {currentCell};
            while (currentCell.pathInfo.previousInPath is { })
            {
                reversedPath.Add(currentCell.pathInfo.previousInPath);
                currentCell = currentCell.pathInfo.previousInPath;
            }
            reversedPath.Reverse();
            reversedPath.RemoveAt(0);
            return reversedPath.ConvertAll(c => c.position);
        }

        private const float HorizontalCost = 1;
        private static readonly float DiagonalCost = Mathf.Sqrt(2);

        //> DISTANCE HEURISTIC FUNCTION
        private static float DistanceBetween(Cell first, Cell second)
        {
            var d = (first.position - second.position).Abs();
            return HorizontalCost * Mathf.Max(d.x, d.y) + (DiagonalCost-HorizontalCost) * Mathf.Min(d.x, d.y);
        }
    }
}