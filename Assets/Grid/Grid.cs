using System;
using System.Linq;
using System.Collections.Generic;
using ProcessControl.Tools;

namespace ProcessControl.Terrain
{
    public class Grid : UnityEngine.MonoBehaviour
    {
        [Serializable] public class Cell
        {
            public Conveyors.Node node;
            public bool occupied => node is { };
            
            public UnityEngine.Vector2Int coordinates;
            public UnityEngine.Vector3 center;
        }

        public UnityEngine.Vector2Int dimensions;
        public List<Cell> cells;

        private UnityEngine.RectInt gridRect;

        public static Func<UnityEngine.Vector2Int, Cell> GetCellCoords;
        public static Func<UnityEngine.Vector3, Cell> GetCellPosition;
        
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
                    // Debug.Log($"Adding cell ({x},{y})");
                    
                    cells.Add(new Cell
                    {
                        center = new UnityEngine.Vector3(x + 0.5f, y + 0.5f),
                        coordinates = new UnityEngine.Vector2Int(x,y),
                    });
                }
            }
        }

        public Cell OnGetCellCoords(UnityEngine.Vector2Int coordinates)
        {
            var cell = cells.FirstOrDefault(c => c.coordinates == coordinates);
            return cell;
        }

        public Cell OnGetCellPosition(UnityEngine.Vector3 worldPosition)
        {
            var coords = new UnityEngine.Vector2Int(worldPosition.x.FloorToInt(), worldPosition.y.FloorToInt());
            // Debug.Log(coords);
            return OnGetCellCoords(coords);
        }
    }
}
