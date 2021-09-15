using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ProcessControl.Conveyors;
using ProcessControl.Tools;
using UnityEngine;

namespace ProcessControl.Terrain
{
    public class ProceduralGrid : MonoBehaviour
    {
        [Serializable] public class Cell
        {
            public bool occupied => node is { };
            public Node node;
            
            public Vector3 center;
            public Vector2Int coordinates;
        }

        public Vector2Int dimensions;
        public List<Cell> cells;

        private RectInt gridRect;

        public static Func<Vector2Int, Cell> GetCellCoords;
        public static Func<Vector3, Cell> GetCellPosition;
        
        private void Awake()
        {
            GetCellCoords += OnGetCellCoords;
            GetCellPosition += OnGetCellPosition;
            
            gridRect.height = dimensions.y;
            gridRect.width = dimensions.x;
            gridRect.x = -dimensions.x / 2;
            gridRect.y = -dimensions.y / 2;

            cells = new List<Cell>();
            
            for (int y = gridRect.y; y < -gridRect.y; y++) {
                for (int x = gridRect.x; x < -gridRect.x; x++)
                {
                    Debug.Log($"Adding cell ({x},{y})");
                    
                    cells.Add(new Cell
                    {
                        center = new Vector3(x + 0.5f, y + 0.5f),
                        coordinates = new Vector2Int(x,y),
                    });
                }
            }
        }

        public Cell OnGetCellCoords(Vector2Int coordinates)
        {
            var cell = cells.FirstOrDefault(c => c.coordinates == coordinates);
            return cell;
        }

        public Cell OnGetCellPosition(Vector3 worldPosition)
        {
            var coords = new Vector2Int(worldPosition.x.FloorToInt(), worldPosition.y.FloorToInt());
            // Debug.Log(coords);
            return OnGetCellCoords(coords);
        }
    }
}
