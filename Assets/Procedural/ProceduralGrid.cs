using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using ProcessControl.Tools;
using ProcessControl.Graphs;
using ProcessControl.Industry.Resources;
using TCM.NoiseGeneration;

#pragma warning disable 108,114


namespace ProcessControl.Procedural
{
    [RequireComponent(typeof(Grid))]
    public class ProceduralGrid : MonoBehaviour
    {
        [Serializable] public class Cell
        {
            public void ResetNode() => node = null;
            
            public Node node;
            public bool occupied => node is { };

            public float value;
            
            public Vector2Int coordinates;
            public Vector3 center;

            public List<(int, Resource.Type)> resourceDeposit = new List<(int, Resource.Type)>();
        }

        public Vector2Int dimensions;
        public Tilemap tilemap;
        public List<TileBase> tiles;
        public List<Cell> cells;

        public Noise.Layer noiseSettings;
        public Range noiseRange;

        private RectInt gridRect;

        private Camera camera;

        public static Func<Cell> GetCellUnderMouse;
        public static Func<Vector2Int, Cell> GetCellCoords;
        public static Func<Vector3, Cell> GetCellPosition;
        
        private void Awake() => Initialize();
        
        private void Initialize()
        {
            camera = Camera.main;
            
            //- register events
            GetCellCoords += OnGetCellCoords;
            GetCellPosition += OnGetCellPosition;
            GetCellUnderMouse += OnGetCellUnderMouse;
            
            //! temp
            gridRect.height = dimensions.y;
            gridRect.width = dimensions.x;
            gridRect.x = -dimensions.x / 2;
            gridRect.y = -dimensions.y / 2;

            cells = new List<Cell>();
            noiseRange = new Range();
            tilemap = GetComponentInChildren<Tilemap>();
            
            for (int y = gridRect.y; y < -gridRect.y; y++) {
                for (int x = gridRect.x; x < -gridRect.x; x++)
                {
                    // Debug.Log($"Adding cell ({x},{y})");
                    
                    cells.Add(new Cell
                    {
                        center = new Vector3(x + 0.5f, y + 0.5f),
                        coordinates = new Vector2Int(x, y),
                    });
                }
            }

            
            GenerateOre();
        }

        public void GenerateOre()
        {
            cells.ForEach(c =>
            {
                var noiseValue = Noise.GenerateValue(noiseSettings, new Vector3(c.coordinates.x, c.coordinates.y, 0f));
                noiseRange.Add(noiseValue);
                c.value = noiseValue;
                var tile = (noiseValue >= 0.5) ? tiles[0] : tiles[1];
                tilemap.SetTile(new Vector3Int(c.coordinates.x, c.coordinates.y, 0), tile);
            });

            Debug.Log($"Noise Range: {noiseRange.min}-{noiseRange.max}");
        }

        //> GET CELLS 
        private Cell OnGetCellUnderMouse()
        {
            var mousePosition = camera.MouseWorldPosition2D();
            return OnGetCellPosition(mousePosition);
        }
        private Cell OnGetCellCoords(Vector2Int coordinates)
        {
            var cell = cells.FirstOrDefault(c => c.coordinates == coordinates);
            return cell;
        }
        private Cell OnGetCellPosition(Vector3 worldPosition)
        {
            var coords = new Vector2Int(worldPosition.x.FloorToInt(), worldPosition.y.FloorToInt());
            // Debug.Log(coords);
            return OnGetCellCoords(coords);
        }
    }
}
