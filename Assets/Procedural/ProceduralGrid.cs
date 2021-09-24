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
        private List<Cell> cells;

        public List<Noise.Layer> noiseLayers;
        public Range noiseRange;

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
                var noiseValue = GenerateNoise(c);
                noiseRange.Add(noiseValue);
                c.value = noiseValue;
                var tile = (noiseValue >= noiseLayers[0].localZero) ? tiles[0] : tiles[1];
                tilemap.SetTile(new Vector3Int(c.coordinates.x, c.coordinates.y, 0), tile);
            });

            // Debug.Log($"Noise Range: {noiseRange.min}-{noiseRange.max}");
        }
        
        //> GET THE ELEVATION AT ANY GIVEN POINT
        public float GenerateNoise(Cell cell)
        {
            float noiseValue = 0f;
            float firstLayerElevation = 0f;

            if (noiseLayers.Count > 0)
            {
                firstLayerElevation = Noise.GenerateValue(noiseLayers[0], cell.center);
                if (noiseLayers[0].enabled) noiseValue = firstLayerElevation;
            }

            for (int i = 1; i < noiseLayers.Count; i++)
            {
                // ignore if not enabled
                if (!noiseLayers[i].enabled) continue;

                float firstLayerMask = (noiseLayers[i].useMask) ? firstLayerElevation : 1;
                noiseValue += Noise.GenerateValue(noiseLayers[i], cell.center) * firstLayerMask;
            }

            noiseRange.Add(noiseValue);
            return noiseValue;
        }

        public void ClearGrid()
        {
            tilemap.ClearAllTiles();
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
