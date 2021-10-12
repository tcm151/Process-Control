using System;
using System.Collections.Generic;
using ProcessControl.Procedural;
using UnityEngine;


namespace ProcessControl.Pathfinding
{
    public static class AStar
    {
        public class Path<T>
        {
            public T cell;
            
            public Vector2Int coordinates;
            
            public int gCost;
            public int hCost;
            public int fCost => gCost + hCost;

            public Path<T> Previous;
        }
        
        private static List<Vector3> FindPath(Cell[,] cells, Vector3 start, Vector3 end)
        {
            var closedList = new List<Cell>();
            var openList = new List<Cell>();

            var startCell = ProceduralGrid.GetCellPosition(start);
            var endCell = ProceduralGrid.GetCellPosition(end);

            foreach (var cell in cells)
            {
                if (cell == startCell)
                {
                    
                }
                
                var pathCell = new Path<Cell>
                {
                    cell = cell,
                    
                    coordinates = cell.coords,
                    gCost = int.MaxValue,
                    hCost = 0, 
                };
            }
            
            return null;
        }
    }
}