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
            public int chunkResolution = 64;
            public int gridResolution = 4;
            
            public List<Tilemap> tilemaps;
            public List<TileBase> tiles;
            
            [Header("Noise")]
            public Range noiseRange;
            public List<Noise.Layer> terrainNoise;
            // public List<Noise.Layer> resourceNoise;
            public List<ResourceNoiseLayer> resourceNoiseLayers;

            [Header("Cells & Chunks")]
            public Cell lastCell;
            public Chunk[,] chunkArray;
        }
        [SerializeField] internal Data grid;

        private Camera camera;
        private readonly Stopwatch timer = new Stopwatch();

        public static Func<Cell> GetCellUnderMouse;
        public static Func<Vector3, Cell> GetCellPosition;
        public static Func<Vector2Int, Cell> GetCellCoords;

        public static Func<Chunk[,]> GetChunkArray;

        //> INITIALIZATION
        public void Awake()
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
                Debug.Log(grid.lastCell.indexes);
            }
        }

        public void Initialize()
        {
            camera = Camera.main;

            //- register events
            GetCellCoords += OnGetCellCoords;
            GetCellPosition += OnGetCellPosition;
            GetCellUnderMouse += OnGetCellUnderMouse;

            GetChunkArray += () => grid.chunkArray;

            grid.terrainNoise.ForEach(t => t.offset = Random.insideUnitSphere * (Random.value * 10));
            grid.resourceNoiseLayers.ForEach(r => r.offset = Random.insideUnitSphere * (Random.value * 2));

            grid.tilemaps = GetComponentsInChildren<Tilemap>().ToList();
            ClearAllTiles();

            grid.chunkArray = new Chunk[grid.gridResolution, grid.gridResolution];
            
            //- creat chunk array
            for (int y = 0; y < grid.gridResolution; y++) {
                for (int x = 0; x < grid.gridResolution; x++)
                {
                    float xOffset = (x - (grid.gridResolution / 2f)) * grid.chunkResolution;
                    float yOffset = (y - (grid.gridResolution / 2f)) * grid.chunkResolution;
                    
                    grid.chunkArray[x, y] = new Chunk
                    {
                        chunkOffset = new Vector2Int(xOffset.FloorToInt(), yOffset.FloorToInt()),
                        neighbours = new Chunk[8],
                        cells = new Cell[grid.chunkResolution, grid.chunkResolution],
                    };
                    // Debug.Log($"Chunk offset for [{x},{y}]:  {grid.chunkArray[x, y].chunkOffset}");
                }
            }
            
            //- assign chunk neighbours
            for (int y = 0; y < grid.gridResolution; y++) {
                for (int x = 0; x < grid.gridResolution; x++)
                {
                    if (x - 1 >= 0 && y + 1 < grid.gridResolution) grid.chunkArray[x, y].neighbours[0] = grid.chunkArray[x - 1, y + 1];
                    if (y + 1 < grid.gridResolution) grid.chunkArray[x, y].neighbours[1] = grid.chunkArray[x, y + 1];
                    if (x + 1 < grid.gridResolution && y + 1 < grid.gridResolution) grid.chunkArray[x, y].neighbours[2] = grid.chunkArray[x + 1, y + 1];

                    if (x - 1 >= 0) grid.chunkArray[x, y].neighbours[3] = grid.chunkArray[x - 1, y];
                    if (x + 1 < grid.gridResolution) grid.chunkArray[x, y].neighbours[4] = grid.chunkArray[x + 1, y];

                    if (x - 1 >= 0 && y - 1 >= 0) grid.chunkArray[x, y].neighbours[5] = grid.chunkArray[x - 1, y - 1];
                    if (y - 1 >= 0) grid.chunkArray[x, y].neighbours[6] = grid.chunkArray[x, y - 1];
                    if (x + 1 < grid.gridResolution && y - 1 >= 0) grid.chunkArray[x, y].neighbours[7] = grid.chunkArray[x + 1, y - 1];
                }
            }
            
            //- create cell arrays
            foreach (var chunk in grid.chunkArray)
            {
                for (int y = 0; y < grid.chunkResolution; y++) {
                    for (int x = 0; x < grid.chunkResolution; x++)
                    {
                        chunk.cells[x, y] = new Cell
                        {
                            parentChunk = chunk,

                            position = new Vector3(x + chunk.chunkOffset.x + 0.5f, y + chunk.chunkOffset.y + 0.5f),
                            coords = new Vector2Int(x + chunk.chunkOffset.x, y + chunk.chunkOffset.y),
                            indexes = new Vector2Int(x, y),
                        };
                    }
                }

                
            }
            
            foreach (var chunk in grid.chunkArray)
            {
                for (int y = 0; y < grid.chunkResolution; y++) {
                    for (int x = 0; x < grid.chunkResolution; x++)
                    {
                        if (x - 1 >= 0 && y + 1 < grid.chunkResolution) chunk.cells[x, y].neighbours[0] = chunk.cells[x - 1, y + 1];
                        if (y + 1 < grid.chunkResolution) chunk.cells[x, y].neighbours[1] = chunk.cells[x, y + 1];
                        if (x + 1 < grid.chunkResolution && y + 1 < grid.chunkResolution) chunk.cells[x, y].neighbours[2] = chunk.cells[x + 1, y + 1];

                        if (x - 1 >= 0) chunk.cells[x, y].neighbours[3] = chunk.cells[x - 1, y];
                        if (x + 1 < grid.chunkResolution) chunk.cells[x, y].neighbours[4] = chunk.cells[x + 1, y];

                        if (x - 1 >= 0 && y - 1 >= 0) chunk.cells[x, y].neighbours[5] = chunk.cells[x - 1, y - 1];
                        if (y - 1 >= 0) chunk.cells[x, y].neighbours[6] = chunk.cells[x, y - 1];
                        if (x + 1 < grid.chunkResolution && y - 1 >= 0) chunk.cells[x, y].neighbours[7] = chunk.cells[x + 1, y - 1];

                        //! corners
                        if (chunk.neighbours[0] is { } && x - 1 == -1 && y + 1 == grid.chunkResolution) chunk.cells[x, y].neighbours[0] = chunk.neighbours[0].cells[grid.chunkResolution-1, 0];
                        if (chunk.neighbours[2] is { } && x + 1 == grid.chunkResolution && y + 1 == grid.chunkResolution) chunk.cells[x, y].neighbours[2] = chunk.neighbours[2].cells[0, 0];
                        if (chunk.neighbours[5] is { } && x - 1 == -1 && y - 1 == -1) chunk.cells[x, y].neighbours[5] = chunk.neighbours[5].cells[grid.chunkResolution - 1, grid.chunkResolution - 1];
                        if (chunk.neighbours[7] is { } && x + 1 == grid.chunkResolution && y - 1 == -1) chunk.cells[x, y].neighbours[7] = chunk.neighbours[7].cells[0, grid.chunkResolution - 1];
                        
                        //! top
                        if (chunk.neighbours[1] is { } && y + 1 == grid.chunkResolution)
                        {
                            chunk.cells[x, y].neighbours[1] = chunk.neighbours[1].cells[x, 0];
                            if (x > 0) chunk.cells[x, y].neighbours[0] = chunk.neighbours[1].cells[x-1, 0];
                            if (x < grid.chunkResolution-1) chunk.cells[x, y].neighbours[2] = chunk.neighbours[1].cells[x+1, 0];
                        }

                        //! left
                        if (chunk.neighbours[3] is { } && x - 1 == -1)
                        {
                            chunk.cells[x, y].neighbours[3] = chunk.neighbours[3].cells[grid.chunkResolution - 1, y];
                            if (y > 0) chunk.cells[x, y].neighbours[5] = chunk.neighbours[3].cells[grid.chunkResolution-1, y-1];
                            if (y < grid.chunkResolution-1) chunk.cells[x, y].neighbours[0] = chunk.neighbours[3].cells[grid.chunkResolution-1, y+1];
                        }
                        
                        //! right
                        if (chunk.neighbours[4] is { } && x + 1 == grid.chunkResolution)
                        {
                            chunk.cells[x, y].neighbours[4] = chunk.neighbours[4].cells[0, y];
                            if (y > 0) chunk.cells[x, y].neighbours[7] = chunk.neighbours[4].cells[0, y - 1];
                            if (y < grid.chunkResolution - 1) chunk.cells[x, y].neighbours[2] = chunk.neighbours[4].cells[0, y+1];
                        }
                        
                        //! bottom
                        if (chunk.neighbours[6] is { } && y - 1 == -1)
                        {
                            chunk.cells[x, y].neighbours[6] = chunk.neighbours[6].cells[x, grid.chunkResolution - 1];
                            if (x < grid.chunkResolution - 1) chunk.cells[x, y].neighbours[7] = chunk.neighbours[6].cells[x+1, grid.chunkResolution-1];
                            if (x > 0) chunk.cells[x, y].neighbours[5] = chunk.neighbours[6].cells[x-1, grid.chunkResolution-1];
                        }
                    }
                }
            }
        }

        //> GENERATE CHUNKS
        public void GenerateAllChunks() => GenerateChunks(grid.chunkArray);
        // private void GenerateChunks(List<Chunk> chunks)
        private void GenerateChunks(Chunk[,] chunks)
        {
            // multi-threaded chunk generation
            var tasks = new List<Task>();
            foreach (var c in chunks) tasks.Add(Task.Factory.StartNew(() => GenerateCells(c.cells)));
            // chunks.ForEach(c => tasks.Add(Task.Factory.StartNew(() => GenerateCells(c.cells))));
            Task.WaitAll(tasks.ToArray());
            
            // apply triangulations on main thread
            // chunks.ForEach(c => UpdateTerrainTiles(0, c.cells));
            foreach (var c in chunks) UpdateTerrainTiles(0, c.cells);
        }

        //> GENERATE CELLS
        // public void GenerateAllCells() => grid.chunks.ForEach(c => GenerateCells(c.cells));
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
        // public void GenerateAllResources() => grid.chunks.ForEach(c => GenerateResources(c.cells));
        public void GenerateAllResources() { foreach (var c in grid.chunkArray) GenerateResources(c.cells);}
        public void GenerateResources(Cell[,] cells)
        {
            foreach (var cell in cells)
            {
                cell.resourceDeposits.Clear();
                
                grid.resourceNoiseLayers.ForEach(rnl =>
                {
                    var noiseValue = Noise.GenerateValue(rnl, cell.position);
                    if (noiseValue >= rnl.threshold && cell.buildable)
                    {
                        cell.resourceDeposits.Add(new ResourceDeposit
                        {
                            noiseValue = noiseValue,
                            quantity = (noiseValue * 16484f).FloorToInt(),
                            material = rnl.resource.material,
                            type = rnl.resource.form,
                        });
                    }
                });
                
                TileBase tile;
                if (cell.resourceDeposits.Count == 0) tile = grid.tiles[3];
                else
                {
                    tile = (cell.resourceDeposits[0].material) switch
                    {
                        ResourceProperties.Material.Iron     => grid.tiles[4],
                        ResourceProperties.Material.Gold     => grid.tiles[6],
                        ResourceProperties.Material.Coal     => grid.tiles[8],
                        ResourceProperties.Material.Copper   => grid.tiles[2],
                        ResourceProperties.Material.Platinum => grid.tiles[7],
                                    _                        => grid.tiles[3],
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
            // var cells = grid.chunks.SelectMany(chunk => chunk.cells.To2D());
            var cells = grid.chunkArray.To2D().SelectMany(c => c.cells.To2D());
            var cell = cells.FirstOrDefault(cell => cell.coords == coordinates);

            return cell;
        }

        //> DRAW HELPFUL GIZMOS
        private void OnDrawGizmos()
        {
            if (grid.lastCell is null) return;
         
            // Gizmos.color = Color.red;
            for (int i = 0; i < grid.lastCell.neighbours.Length; i++)
            {
                if (grid.lastCell.neighbours[i] is null) continue;
        
                Gizmos.color = Color.Lerp(Color.red, Color.black, i / 7f);
                Gizmos.DrawSphere(grid.lastCell.neighbours[i].position, 0.25f);
            }
            
            // Gizmos.color = Color.green;
            for (int i = 0; i < grid.lastCell.parentChunk.neighbours.Length; i++)
            {
                if (grid.lastCell.parentChunk.neighbours[i] is null) continue;
                
                Gizmos.color = Color.Lerp(Color.green, Color.black, i / 7f);
                var chunkOffset = grid.lastCell.parentChunk.neighbours[i].chunkOffset.ToVector3();
                chunkOffset.x += grid.chunkResolution / 2f;
                chunkOffset.y += grid.chunkResolution / 2f;
                Gizmos.DrawSphere(chunkOffset, grid.chunkResolution * 0.5f);
            }
        }
    }

    [Serializable] public class ResourceNoiseLayer : Noise.Layer
    {
        public ResourceProperties resource;
    }
}
