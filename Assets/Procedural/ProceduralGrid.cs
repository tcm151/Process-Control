using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using ProcessControl.Tools;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

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
            
            [FormerlySerializedAs("tilemap")]
            public List<Tilemap> tilemaps;
            public List<TileBase> tiles;
            
            [Header("Noise")]
            public Range noiseRange;
            public List<Noise.Layer> terrainNoise;
            [FormerlySerializedAs("oreNoise")] public List<Noise.Layer> resourceNoise;

            [Header("Cells & Chunks")]
            public Cell lastCell;
            public List<Chunk> chunks = new List<Chunk>();
        }
        [SerializeField] internal Data grid;

        
        private Camera camera;

        public static Func<Cell> GetCellUnderMouse;
        public static Func<Vector3, Cell> GetCellPosition;
        public static Func<Vector2Int, Cell> GetCellCoords;

        private void Awake()
        {
            Initialize();
            GenerateAllChunks();
            GenerateAllResources();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                grid.lastCell = GetCellUnderMouse();
            }
        }

        public void Initialize()
        {
            camera = Camera.main;

            //- register events
            GetCellCoords += OnGetCellCoords;
            GetCellPosition += OnGetCellPosition;
            GetCellUnderMouse += OnGetCellUnderMouse;

            grid.chunks.Clear();
            
            grid.terrainNoise.ForEach(t => t.offset = Random.insideUnitSphere * (Random.value * 10));
            grid.resourceNoise.ForEach(r => r.offset = Random.insideUnitSphere * (Random.value * 2));

            grid.tilemaps = GetComponentsInChildren<Tilemap>().ToList();

            //- create new chunks
            for (int y = -grid.gridDimensions.y / 2; y <= grid.gridDimensions.y / 2; y++) {
                for (int x = -grid.gridDimensions.x / 2; x <= grid.gridDimensions.x / 2; x++)
                {
                    grid.chunks.Add(new Chunk
                    {
                        chunkOffset = new Vector2Int(x * grid.chunkDimensions.x, y * grid.chunkDimensions.y)
                    });
                }
            }
            
            //- create cell arrays
            grid.chunks.ForEach(c =>
            {
                c.cells = new Cell[grid.chunkDimensions.x, grid.chunkDimensions.y];
                for (int y = 0; y < grid.chunkDimensions.y; y++) {
                    for (int x = 0; x < grid.chunkDimensions.x; x++)
                    {
                        c.cells[x,y] = new Cell
                        {
                            position = new Vector3(x + c.chunkOffset.x + 0.5f, y + c.chunkOffset.y + 0.5f),
                            coordinates = new Vector2Int(x + c.chunkOffset.x, y + c.chunkOffset.y),
                        };
                    }
                }
            });
        }

        //> GENERATE CHUNKS
        public void GenerateAllChunks() => GenerateChunks(grid.chunks);
        private void GenerateChunks(List<Chunk> chunks)
        {
            // queue tasks
            // Task[] triangulations = new Task[chunks.Count];
            // for (int i = 0; i < chunks.Count; i++)
            // {
            //     int j = i;
            //     triangulations[j] = Task.Factory.StartNew(() => GenerateCells(chunks[j].cells));
            // }
            // Task.WaitAll(triangulations, 5000);

            var tasks = new List<Task>();
            chunks.ForEach(c => tasks.Add(Task.Factory.StartNew(() => GenerateCells(c.cells))));
            Task.WaitAll(tasks.ToArray());

            // apply triangulations on main thread
            chunks.ForEach(c => UpdateTerrainTiles(0, c.cells));
        }

        public void GenerateAllCells() => grid.chunks.ForEach(c => GenerateCells(c.cells));
        public void GenerateCells(Cell[,] cells)
        {
            foreach (var cell in cells)
            {
                var noiseValue = GenerateNoise(grid.terrainNoise, cell);
                grid.noiseRange.Add(noiseValue);
                cell.terrainValue = noiseValue;
            }
        }

        public void GenerateAllResources() => grid.chunks.ForEach(c => GenerateOre(c.cells));
        public void GenerateOre(Cell[,] cells)
        {
            foreach (var cell in cells)
            {
                var noiseValue = GenerateNoise(grid.resourceNoise, cell);
                cell.resourceValue = noiseValue;

                if (cell.resourceValue >= grid.resourceNoise[0].threshold)
                {
                    cell.resourceDeposit = (cell.resourceValue * 1024f).FloorToInt();
                }
            }

            foreach (var cell in cells)
            {
                var tile = (cell.resourceValue >= grid.resourceNoise[0].threshold && cell.buildable) ? grid.tiles[2] : grid.tiles[3];
                grid.tilemaps[1].SetTile(new Vector3Int(cell.coordinates.x, cell.coordinates.y, 0), tile);
                // grid.tilemaps[1].RefreshAllTiles();
            }
        }

        //> TILE MODIFICATION
        public void ClearTiles() => grid.tilemaps.ForEach(t => t.ClearAllTiles());
        public void UpdateTerrainTiles(int map, Cell[,] cells)
        {
            foreach (var cell in cells)
            {
                if (cell.terrainValue < grid.terrainNoise[0].threshold) cell.buildable = false;

                var tile = (cell.terrainValue >= grid.terrainNoise[0].threshold) ? grid.tiles[1] : grid.tiles[0];
                grid.tilemaps[map].SetTile(new Vector3Int(cell.coordinates.x, cell.coordinates.y, 0), tile);
            }
        }
        
        //> GET A NOISE VALUE FOR ANY SPECIFIC CELL
         public static float GenerateNoise(List<Noise.Layer> noiseLayers, Cell cell)
         {
             float noiseValue = 0f;
             float firstLayerElevation = 0f;

             //- calculate first layer
             if (noiseLayers.Count > 0)
             {
                 firstLayerElevation = Noise.GenerateValue(noiseLayers[0], cell.position);
                 if (noiseLayers[0].enabled) noiseValue = firstLayerElevation;
             }

             //- calculate every other layer
             for (int i = 1; i < noiseLayers.Count; i++)
             {
                 // ignore if not enabled
                 if (!noiseLayers[i].enabled) continue;

                 float firstLayerMask = (noiseLayers[i].useMask) ? firstLayerElevation : 1;
                 noiseValue += Noise.GenerateValue(noiseLayers[i], cell.position) * firstLayerMask;
             }

             return noiseValue;
         }

        //> GET CELLS 
        private Cell OnGetCellUnderMouse()
        {
            Vector2 position = camera.MousePosition2D();
            return OnGetCellCoords(position.FloorToInt());
        }
        private Cell OnGetCellPosition(Vector3 worldPosition) => OnGetCellCoords(new Vector2Int(worldPosition.x.FloorToInt(), worldPosition.y.FloorToInt()));
        private Cell OnGetCellCoords(Vector2Int coordinates)
        {
            var cells = grid.chunks.SelectMany(chunk => chunk.cells.To2D());
            var cell = cells.FirstOrDefault(cell => cell.coordinates == coordinates);

            // var cells = noiseLayers.chunks.ConvertAll(chunk => chunk.cells[coordinates.x, coordinates.y]);
            // var cell = cells.FirstOrDefault(cell => cell.coordinates == coordinates);
            
            return cell;
        }
    }
}
