using System;
using System.Collections.Generic;
using ProcessControl.Procedural;
using UnityEngine;


namespace ProcessControl.Pathfinding
{
    public static class AStar
    {
        public class PathCell
        {
            public Vector2Int coordinates;
            
            public int gCost;
            public int hCost;
            public int fCost => gCost + hCost;

            public PathCell previousCell;
        }
        
        private static List<Vector3> FindPath(Cell[,] cells, Vector3 start, Vector3 end)
        {
            var closedList = new List<Cell>();
            var openList = new List<Cell>();

            var startCell = ProceduralGrid.GetCellPosition(start);
            var endCell = ProceduralGrid.GetCellPosition(end);

            foreach (var cell in cells)
            {
                var pathCell = new PathCell
                {
                    coordinates = cell.coords,
                    gCost = Int32.MaxValue,
                    hCost = 
                };
            }
            
            return null;
        }
    }
}