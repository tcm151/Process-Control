using System;
using System.Linq;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;
using ProcessControl.Tools;
using ProcessControl.Industry.Resources;
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
            
            public List<Tilemap> tilemaps;
            public List<TileBase> tiles;
            
            [Header("Noise")]
            public Range noiseRange;
            public List<Noise.Layer> terrainNoise;
            public List<Noise.Layer> resourceNoise;

            [Header("Cells & Chunks")]
            public Cell lastCell;
            public List<Chunk> chunks = new List<Chunk>();
        }
        [SerializeField] internal Data grid;

        private Camera camera;
        private readonly Stopwatch timer = new Stopwatch();

        public static Func<Cell> GetCellUnderMouse;
        public static Func<Vector3, Cell> GetCellPosition;
        public static Func<Vector2Int, Cell> GetCellCoords;

        public static Func<List<Chunk>> GetChunks;

        //> INITIALIZATION
        private void Awake()
        {
            timer.Start();
            Initialize();
            float init = timer.ElapsedMilliseconds;
            timer.Restart();
            GenerateAllChunks();
            float chunkGen = timer.ElapsedMilliseconds;
            timer.Restart();
            GenerateAllResources();
            float resourceGen = timer.ElapsedMilliseconds;
            timer.Reset();
            
            Debug.Log($"{init} | {chunkGen} | {resourceGen} |= {init+chunkGen+resourceGen} ms");
        }

        //> CACHE LAST TOUCHED CELL
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

            GetChunks += () => grid.chunks;

            grid.chunks.Clear();
            
            grid.terrainNoise.ForEach(t => t.offset = Random.insideUnitSphere * (Random.value * 10));
            grid.resourceNoise.ForEach(r => r.offset = Random.insideUnitSphere * (Random.value * 2));

            grid.tilemaps = GetComponentsInChildren<Tilemap>().ToList();
            ClearAllTiles();

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
                            coords = new Vector2Int(x + c.chunkOffset.x, y + c.chunkOffset.y),
                        };

                        if (x - 1 >= 0 && y + 1 < grid.chunkDimensions.y) c.cells[x, y].neighbours[0] = c.cells[x - 1, y + 1];
                        if (y + 1 < grid.chunkDimensions.y) c.cells[x, y].neighbours[1] = c.cells[x, y + 1];
                        if (x + 1 < grid.chunkDimensions.x && y + 1 < grid.chunkDimensions.y) c.cells[x, y].neighbours[2] = c.cells[x + 1, y + 1];
                        
                        if (x - 1 >= 0) c.cells[x, y].neighbours[3] = c.cells[x - 1, y];
                        if (x + 1 < grid.chunkDimensions.x) c.cells[x, y].neighbours[4] = c.cells[x + 1, y];
                        
                        if (x - 1 >= 0 && y - 1 >= 0 ) c.cells[x, y].neighbours[5] = c.cells[x - 1, y - 1];
                        if (y - 1 >= 0) c.cells[x, y].neighbours[6] = c.cells[x, y - 1];
                        if (x + 1 < grid.chunkDimensions.x && y - 1 >= 0) c.cells[x, y].neighbours[7] = c.cells[x + 1, y - 1];
                    }
                }
                
                for (int y = 0; y < grid.chunkDimensions.y; y++) {
                    for (int x = 0; x < grid.chunkDimensions.x; x++)
                    {
                        // c.cells[x,y] = new Cell
                        // {
                        //     position = new Vector3(x + c.chunkOffset.x + 0.5f, y + c.chunkOffset.y + 0.5f),
                        //     coords = new Vector2Int(x + c.chunkOffset.x, y + c.chunkOffset.y),
                        // };

                        if (x - 1 >= 0 && y + 1 < grid.chunkDimensions.y) c.cells[x, y].neighbours[0] = c.cells[x - 1, y + 1];
                        if (y + 1 < grid.chunkDimensions.y) c.cells[x, y].neighbours[1] = c.cells[x, y + 1];
                        if (x + 1 < grid.chunkDimensions.x && y + 1 < grid.chunkDimensions.y) c.cells[x, y].neighbours[2] = c.cells[x + 1, y + 1];
                        
                        if (x - 1 >= 0) c.cells[x, y].neighbours[3] = c.cells[x - 1, y];
                        if (x + 1 < grid.chunkDimensions.x) c.cells[x, y].neighbours[4] = c.cells[x + 1, y];
                        
                        if (x - 1 >= 0 && y - 1 >= 0 ) c.cells[x, y].neighbours[5] = c.cells[x - 1, y - 1];
                        if (y - 1 >= 0) c.cells[x, y].neighbours[6] = c.cells[x, y - 1];
                        if (x + 1 < grid.chunkDimensions.x && y - 1 >= 0) c.cells[x, y].neighbours[7] = c.cells[x + 1, y - 1];
                    }
                }
            });
        }

        //> GENERATE CHUNKS
        public void GenerateAllChunks() => GenerateChunks(grid.chunks);
        private void GenerateChunks(List<Chunk> chunks)
        {
            // multi-threaded chunk generation
            var tasks = new List<Task>();
            chunks.ForEach(c => tasks.Add(Task.Factory.StartNew(() => GenerateCells(c.cells))));
            Task.WaitAll(tasks.ToArray());
            
            // apply triangulations on main thread
            chunks.ForEach(c => UpdateTerrainTiles(0, c.cells));
        }

        //> GENERATE CELLS
        public void GenerateAllCells() => grid.chunks.ForEach(c => GenerateCells(c.cells));
        public void GenerateCells(Cell[,] cells)
        {
            foreach (var cell in cells)
            {
                var noiseValue = Noise.GenerateValue(grid.terrainNoise, cell.position);
                grid.noiseRange.Add(noiseValue);
                cell.terrainValue = noiseValue;
            }
        }

        //> GENERATE RESOURCES
        public void GenerateAllResources() => grid.chunks.ForEach(c => GenerateResources(c.cells));
        public void GenerateResources(Cell[,] cells)
        {
            foreach (var cell in cells)
            {
                cell.resourceDeposits.Clear();
                
                //@ refactor this into a ForEach
                var noiseValue = Noise.GenerateValue(grid.resourceNoise[0], cell.position);
                if (noiseValue >= grid.resourceNoise[0].threshold && cell.buildable)
                {
                    cell.resourceDeposits.Add(new ResourceDeposit
                    {
                        noiseValue = noiseValue,
                        quantity = (noiseValue * 2048f).FloorToInt(),
                        material = ResourceProperties.Material.Copper,
                        type = ResourceProperties.Form.Raw,
                    });
                }
                
                noiseValue = Noise.GenerateValue(grid.resourceNoise[1], cell.position);
                if (noiseValue >= grid.resourceNoise[1].threshold && cell.buildable)
                {
                    cell.resourceDeposits.Add(new ResourceDeposit
                    {
                        noiseValue = noiseValue,
                        quantity = (noiseValue * 2048f).FloorToInt(),
                        material = ResourceProperties.Material.Iron,
                        type = ResourceProperties.Form.Raw,
                    });
                }
                
                noiseValue = Noise.GenerateValue(grid.resourceNoise[2], cell.position);
                if (noiseValue >= grid.resourceNoise[2].threshold && cell.buildable)
                {
                    cell.resourceDeposits.Add(new ResourceDeposit
                    {
                        noiseValue = noiseValue,
                        quantity = (noiseValue * 2048f).FloorToInt(),
                        material = ResourceProperties.Material.Gold,
                        type = ResourceProperties.Form.Raw,
                    });
                }
                
                noiseValue = Noise.GenerateValue(grid.resourceNoise[3], cell.position);
                if (noiseValue >= grid.resourceNoise[3].threshold && cell.buildable)
                {
                    cell.resourceDeposits.Add(new ResourceDeposit
                    {
                        noiseValue = noiseValue,
                        quantity = (noiseValue * 2048f).FloorToInt(),
                        material = ResourceProperties.Material.Platinum,
                        type = ResourceProperties.Form.Raw,
                    });
                }
                
                TileBase tile;
                if (cell.resourceDeposits.Count == 0) tile = grid.tiles[3];
                else
                {
                    tile = (cell.resourceDeposits[0].material) switch
                    {
                        ResourceProperties.Material.Iron     => grid.tiles[4],
                        ResourceProperties.Material.Gold     => grid.tiles[6],
                        ResourceProperties.Material.Copper   => grid.tiles[2],
                        ResourceProperties.Material.Platinum => grid.tiles[7],
                                    _              => grid.tiles[3],
                    };
                }
                grid.tilemaps[1].SetTile(new Vector3Int(cell.coords.x, cell.coords.y, 0), tile);
            }
        }

        //> TILE MODIFICATION
        public void ClearAllTiles() => grid.tilemaps.ForEach(t => t.ClearAllTiles());
        public void UpdateTerrainTiles(int map, Cell[,] cells)
        {
            foreach (var cell in cells)
            {
                if (cell.terrainValue < grid.terrainNoise[0].threshold) cell.buildable = false;

                var tile = (cell.terrainValue >= grid.terrainNoise[0].threshold) ? grid.tiles[1] : grid.tiles[0];
                grid.tilemaps[map].SetTile(new Vector3Int(cell.coords.x, cell.coords.y, 0), tile);
            }
        }
        
        // //> GET A NOISE VALUE FOR ANY SPECIFIC CELL
        // public static float GenerateNoise(List<Noise.Layer> noiseLayers, Cell cell)
        // {
        //     float noiseValue = 0f;
        //     float firstLayerValue = 0f;
        //
        //     //- calculate first layer
        //     if (noiseLayers.Count > 0)
        //     {
        //      firstLayerValue = Noise.GenerateValue(noiseLayers[0], cell.position);
        //      if (noiseLayers[0].enabled) noiseValue = firstLayerValue;
        //     }
        //
        //     //- calculate every other layer
        //     for (int i = 1; i < noiseLayers.Count; i++)
        //     {
        //      // ignore if not enabled
        //      if (!noiseLayers[i].enabled) continue;
        //
        //      float firstLayerMask = (noiseLayers[i].useMask) ? firstLayerValue : 1;
        //      noiseValue += Noise.GenerateValue(noiseLayers[i], cell.position) * firstLayerMask;
        //     }
        //
        //     return noiseValue;
        // }
        //
        // public static float GenerateNoise(Noise.Layer layer, Cell cell, float maskValue = 1)
        // {
        //     if (!layer.enabled) return 0;
        //     return Noise.GenerateValue(layer, cell.position) * maskValue;
        // }

        //> GET CELLS 
        private Cell OnGetCellUnderMouse()
        {
            Vector2 position = camera.MousePosition2D();
            return OnGetCellCoords(position.FloorToInt());
        }
        private Cell OnGetCellPosition(Vector3 worldPosition)
        {
            var coords = new Vector2Int(worldPosition.x.FloorToInt(), worldPosition.y.FloorToInt());
            return OnGetCellCoords(coords);
        }
        private Cell OnGetCellCoords(Vector2Int coordinates)
        {
            var cells = grid.chunks.SelectMany(chunk => chunk.cells.To2D());
            var cell = cells.FirstOrDefault(cell => cell.coords == coordinates);

            return cell;
        }

        private void OnDrawGizmos()
        {
            if (grid.lastCell is null) return;
         
            Gizmos.color = Color.red;
            for (int i = 0; i < grid.lastCell.neighbours.Length; i++)
            {
                if (grid.lastCell.neighbours[i] is null) continue;

                Gizmos.color = Color.Lerp(Color.black, Color.white, i / 7f);
                Gizmos.DrawSphere(grid.lastCell.neighbours[i].position, 0.25f);
            }
        }
    }
}
