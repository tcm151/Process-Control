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
        [Serializable]
        public class Data
        {
            public Vector2Int dimensions;
            public Tilemap tilemap;
            public List<TileBase> tiles;
            [HideInInspector] public List<Cell> cells;
            [HideInInspector] public List<Chunk> chunks;

            public List<Noise.Layer> noiseLayers;
            public Range noiseRange;
        }

        [SerializeField] internal Data grid;
        
        
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

        private RectInt gridRect;

        private Camera camera;

        public static Func<Cell> GetCellUnderMouse;
        public static Func<Vector2Int, Cell> GetCellCoords;
        public static Func<Vector3, Cell> GetCellPosition;
        
        private void Awake() => Initialize();
        
        public void Initialize()
        {
            camera = Camera.main;
            
            //- register events
            GetCellCoords += OnGetCellCoords;
            GetCellPosition += OnGetCellPosition;
            GetCellUnderMouse += OnGetCellUnderMouse;
            
            //! temp
            gridRect.height = grid.dimensions.y;
            gridRect.width = grid.dimensions.x;
            gridRect.x = -grid.dimensions.x / 2;
            gridRect.y = -grid.dimensions.y / 2;

            grid.cells = new List<Cell>();
            grid.noiseRange = new Range();
            grid.tilemap = GetComponentInChildren<Tilemap>();
            
            for (int y = gridRect.y; y < -gridRect.y; y++) {
                for (int x = gridRect.x; x < -gridRect.x; x++)
                {
                    // Debug.Log($"Adding cell ({x},{y})");
                    
                    grid.cells.Add(new Cell
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
            grid.cells.ForEach(c =>
            {
                var noiseValue = GenerateNoise(c);
                grid.noiseRange.Add(noiseValue);
                c.value = noiseValue;
                var tile = (noiseValue >= grid.noiseLayers[0].localZero) ? grid.tiles[0] : grid.tiles[1];
                grid.tilemap.SetTile(new Vector3Int(c.coordinates.x, c.coordinates.y, 0), tile);
            });

            // Debug.Log($"Noise Range: {noiseRange.min}-{noiseRange.max}");
        }
        
        //> GET THE ELEVATION AT ANY GIVEN POINT
        public float GenerateNoise(Cell cell)
        {
            float noiseValue = 0f;
            float firstLayerElevation = 0f;

            if (grid.noiseLayers.Count > 0)
            {
                firstLayerElevation = Noise.GenerateValue(grid.noiseLayers[0], cell.center);
                if (grid.noiseLayers[0].enabled) noiseValue = firstLayerElevation;
            }

            for (int i = 1; i < grid.noiseLayers.Count; i++)
            {
                // ignore if not enabled
                if (!grid.noiseLayers[i].enabled) continue;

                float firstLayerMask = (grid.noiseLayers[i].useMask) ? firstLayerElevation : 1;
                noiseValue += Noise.GenerateValue(grid.noiseLayers[i], cell.center) * firstLayerMask;
            }

            grid.noiseRange.Add(noiseValue);
            return noiseValue;
        }

        public void ClearGrid()
        {
            grid.tilemap.ClearAllTiles();
        }

        //> GET CELLS 
        private Cell OnGetCellUnderMouse()
        {
            var mousePosition = camera.MouseWorldPosition2D();
            return OnGetCellPosition(mousePosition);
        }
        private Cell OnGetCellCoords(Vector2Int coordinates)
        {
            var cell = grid.cells.FirstOrDefault(c => c.coordinates == coordinates);
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
