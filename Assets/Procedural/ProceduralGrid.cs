using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using ProcessControl.Tools;

#pragma warning disable 108, 114


namespace ProcessControl.Procedural
{
    [RequireComponent(typeof(Grid))]
    public class ProceduralGrid : MonoBehaviour
    {
        [Serializable]
        public class Data
        {
            [Header("Resolution")]
            public Vector2Int gridDimensions;
            public Vector2Int chunkDimensions;
            
            public Tilemap tilemap;
            public List<TileBase> tiles;
            
            public List<Noise.Layer> noiseLayers;
            public Range noiseRange;
            
            // public List<Cell> cells = new List<Cell>();
            public List<Chunk> chunks = new List<Chunk>();
            // [HideInInspector] public List<Chunk> chunks = new List<Chunk>();

        }
        [SerializeField] internal Data grid;

        // private RectInt gridRect;
        private Camera camera;

        public static Func<Cell> GetCellUnderMouse;
        public static Func<Vector3, Cell> GetCellPosition;
        public static Func<Vector2Int, Cell> GetCellCoords;

        private void Awake()
        {
            Initialize();
            GenerateAllChunks();
        }

        public void Initialize()
        {
            camera = Camera.main;

            //- register events
            GetCellCoords += OnGetCellCoords;
            GetCellPosition += OnGetCellPosition;
            GetCellUnderMouse += OnGetCellUnderMouse;

            grid.chunks.Clear();

            grid.tilemap = GetComponentInChildren<Tilemap>();

            for (int y = -grid.gridDimensions.y / 2; y <= grid.gridDimensions.y / 2; y++) {
                for (int x = -grid.gridDimensions.x / 2; x <= grid.gridDimensions.x / 2; x++)
                {
                    grid.chunks.Add(new Chunk
                    {
                        chunkOffset = new Vector2Int(x * grid.chunkDimensions.x, y * grid.chunkDimensions.y)
                    });
                }
            }
            
            grid.chunks.ForEach(c =>
            {
                for (int y = -grid.chunkDimensions.y / 2; y < grid.chunkDimensions.y / 2; y++) {
                    for (int x = -grid.chunkDimensions.x / 2; x < grid.chunkDimensions.x / 2; x++)
                    {
                        c.cells.Add(new Cell
                        {
                            center = new Vector3(x + c.chunkOffset.x+ 0.5f, y + c.chunkOffset.y + 0.5f),
                            coordinates = new Vector2Int(x + c.chunkOffset.x, y + c.chunkOffset.y),
                        });
                    }
                }
            });
        }

        public void GenerateAllChunks() => GenerateChunks(grid.chunks);

        private void GenerateChunks(List<Chunk> chunks)
        {
            Task[] triangulations = new Task[chunks.Count];
            for (int i = 0; i < chunks.Count; i++)
            {
                int j = i;
                triangulations[j] = Task.Factory.StartNew(() => GenerateCells(chunks[j].cells));
            }
            Task.WaitAll(triangulations, 5000);

            // apply triangulations on main thread
            chunks.ForEach(c => SetTilemap(c.cells));
        }
        
        public void GenerateCells(List<Cell> cells) => cells.ForEach(c =>
        {
            var noiseValue = GenerateNoise(grid, c);
            grid.noiseRange.Add(noiseValue);
            c.value = noiseValue;
        });

        public void SetTilemap(List<Cell> cells) => cells.ForEach(c =>
        {
            if (c.value < grid.noiseLayers[0].localZero)
            {
                c.buildable = false;
            }
            
            var tile = (c.value >= grid.noiseLayers[0].localZero) ? grid.tiles[1] : grid.tiles[0];
            grid.tilemap.SetTile(new Vector3Int(c.coordinates.x, c.coordinates.y, 0), tile);
        });

        //> GET THE ELEVATION AT ANY GIVEN POINT
         public static float GenerateNoise(Data grid, Cell cell)
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

        public void ClearTiles() => grid.tilemap.ClearAllTiles();

        //> GET CELLS 
        private Cell OnGetCellUnderMouse() => OnGetCellPosition(camera.MouseWorldPosition2D());
        private Cell OnGetCellPosition(Vector3 worldPosition) => OnGetCellCoords(new Vector2Int(worldPosition.x.FloorToInt(), worldPosition.y.FloorToInt()));
        private Cell OnGetCellCoords(Vector2Int coordinates)
        {
            var cells = grid.chunks.SelectMany(chunk => chunk.cells);
            var cell = cells.FirstOrDefault(cell => cell.coordinates == coordinates);
            return cell;
        }

    }
}
