using System;
using System.Linq;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;
using UnityEngine.Serialization;

using ProcessControl.Tools;
using ProcessControl.Industry.Resources;

#pragma warning disable 108, 114


namespace ProcessControl.Procedural
{
    [RequireComponent(typeof(Grid))]
    public class TileGrid : MonoBehaviour
    {
        [Serializable]
        public class Data
        {
            [FormerlySerializedAs("chunkResolution")]
            [Header("Resolution")]
            public int chunkSize = 64;
            [FormerlySerializedAs("gridSize")][FormerlySerializedAs("gridResolution")] public int size = 4;
            
            public List<Tilemap> tilemaps;
            public List<TileBase> tiles;
            
            [Header("Noise")]
            public Range noiseRange;
            public List<Noise.Layer> terrainNoise;
            [FormerlySerializedAs("resourceNoiseLayers")] public List<ResourceNoiseLayer> resourceNoise;
            public List<Noise.Layer> biomeNoise;

            [Header("Cells & Chunks")]
            public Chunk[,] chunks;
            public Cell lastCell;
        }
        [SerializeField] internal Data grid;

        public string seed;
        public bool showNeighbours = false;
        
        private Camera camera;
        private readonly Stopwatch timer = new Stopwatch();

        //> EXTERNAL CALLS
        public static Func<Chunk[,]> GetChunks;
        public static Func<Cell> GetCellUnderMouse;
        public static Func<Vector3, Cell> GetCellAtPosition;
        public static Func<Vector2Int, Cell> GetCellAtCoordinates;


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
            GenerateAllBiomes();
            float biomeGen = timer.ElapsedMilliseconds;
            timer.Restart();
            GenerateAllResources();
            float resourceGen = timer.ElapsedMilliseconds;
            timer.Reset();
            
            Debug.Log($"{init} | {chunkGen} | {resourceGen} | {biomeGen} |= {init+chunkGen+resourceGen} ms");
        }

        //> CACHE LAST TOUCHED CELL
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                grid.lastCell = GetCellUnderMouse();
            }
        }

        private Cell GetCellFromCoordinates(Vector2Int coords)
        {
            var cells = GetChunkFromCoordinates(coords).cells.ToList();
            return cells.FirstOrDefault(cell => cell.coords == coords);
        }

        private Chunk GetChunkFromCoordinates(Vector2Int coords)
        {
            var floatCoords = coords.ToVector2();
            float bounds = (grid.size * grid.chunkSize) / 2f;
            var x = floatCoords.x.Remap(-bounds, bounds - 1, 0, grid.size - 0.0001f).FloorToInt();
            var y = floatCoords.y.Remap(-bounds, bounds - 1, 0, grid.size - 0.0001f).FloorToInt();
            
            Debug.Log($"{x},{y}");
            return grid.chunks[x,y];
        }

        public void Initialize()
        {
            camera = Camera.main;

            //- register events
            GetChunks += () => grid.chunks;
            GetCellAtCoordinates += GetCellFromCoordinates;
            GetCellAtPosition += (position) => GetCellFromCoordinates(position.ToVector2().FloorToInt());
            GetCellUnderMouse += () => GetCellFromCoordinates(camera.MousePosition2D().FloorToInt());

            //- random seed noise layers
            if (seed != "") Random.InitState(seed.GetHashCode());
            grid.biomeNoise.ForEach(b => b.offset = Random.insideUnitSphere * (Random.value * 5));
            grid.terrainNoise.ForEach(t => t.offset = Random.insideUnitSphere * (Random.value * 10));
            grid.resourceNoise.ForEach(r => r.offset = Random.insideUnitSphere * (Random.value * 2));

            //- gather and clear tilemaps
            grid.tilemaps = GetComponentsInChildren<Tilemap>().ToList();
            ClearAllTiles();

            
            //- create chunk array
            //@ requires dynamic chunk loading, so maybe can't use Chunk[,] and have to use list
            /// unless you want to create a new chunk array whenever a new chunk is explored
            grid.chunks = new Chunk[grid.size, grid.size];
            for (int y = 0; y < grid.size; y++) {
                for (int x = 0; x < grid.size; x++)
                {
                    float xOffset = (x - (grid.size / 2f)) * grid.chunkSize;
                    float yOffset = (y - (grid.size / 2f)) * grid.chunkSize;
                    grid.chunks[x, y] = new Chunk
                    {
                        chunkOffset = new Vector2Int(xOffset.FloorToInt(), yOffset.FloorToInt()),
                        neighbours = new Chunk[8],
                        cells = new Cell[grid.chunkSize, grid.chunkSize],
                    };
                }
            }
            
            //- assign chunk neighbours
            for (int y = 0; y < grid.size; y++) {
                for (int x = 0; x < grid.size; x++)
                {
                    // top left, top middle, top right
                    if (x - 1 >= 0 && y + 1 < grid.size)        grid.chunks[x, y].neighbours[0] = grid.chunks[x - 1, y + 1];
                    if (y + 1 < grid.size)                      grid.chunks[x, y].neighbours[1] = grid.chunks[  x  , y + 1];
                    if (x + 1 < grid.size && y + 1 < grid.size) grid.chunks[x, y].neighbours[2] = grid.chunks[x + 1, y + 1];

                    // left and right
                    if (x - 1 >= 0)        grid.chunks[x, y].neighbours[3] = grid.chunks[x - 1, y];
                    if (x + 1 < grid.size) grid.chunks[x, y].neighbours[4] = grid.chunks[x + 1, y];

                    // bottom left, bottom middle, bottomRight
                    if (x - 1 >= 0 && y - 1 >= 0)        grid.chunks[x, y].neighbours[5] = grid.chunks[x - 1, y - 1];
                    if (y - 1 >= 0)                      grid.chunks[x, y].neighbours[6] = grid.chunks[  x  , y - 1];
                    if (x + 1 < grid.size && y - 1 >= 0) grid.chunks[x, y].neighbours[7] = grid.chunks[x + 1, y - 1];
                }
            }
            
            //- create cell arrays
            grid.chunks.ForEach(chunk =>
            {
                for (int y = 0; y < grid.chunkSize; y++) {
                    for (int x = 0; x < grid.chunkSize; x++)
                    {
                        chunk.cells[x, y] = new Cell
                        {
                            parentChunk = chunk,
                            position = new Vector3(x + chunk.chunkOffset.x + 0.5f, y + chunk.chunkOffset.y + 0.5f),
                            coords = new Vector2Int(x + chunk.chunkOffset.x, y + chunk.chunkOffset.y),
                            // indexes = new Vector2Int(x, y),
                        };
                    }
                }
            });
            
            //- assign cell neighbours
            grid.chunks.ForEach(chunk =>
            {
                for (int y = 0; y < grid.chunkSize; y++) {
                    for (int x = 0; x < grid.chunkSize; x++)
                    {
                        if (x - 1 >= 0 && y + 1 < grid.chunkSize) chunk.cells[x, y].neighbours[0] = chunk.cells[x - 1, y + 1];
                        if (y + 1 < grid.chunkSize) chunk.cells[x, y].neighbours[1] = chunk.cells[x, y + 1];
                        if (x + 1 < grid.chunkSize && y + 1 < grid.chunkSize) chunk.cells[x, y].neighbours[2] = chunk.cells[x + 1, y + 1];

                        if (x - 1 >= 0) chunk.cells[x, y].neighbours[3] = chunk.cells[x - 1, y];
                        if (x + 1 < grid.chunkSize) chunk.cells[x, y].neighbours[4] = chunk.cells[x + 1, y];

                        if (x - 1 >= 0 && y - 1 >= 0) chunk.cells[x, y].neighbours[5] = chunk.cells[x - 1, y - 1];
                        if (y - 1 >= 0) chunk.cells[x, y].neighbours[6] = chunk.cells[x, y - 1];
                        if (x + 1 < grid.chunkSize && y - 1 >= 0) chunk.cells[x, y].neighbours[7] = chunk.cells[x + 1, y - 1];

                        //! corners
                        if (chunk.neighbours[0] is { } && x - 1 == -1 && y + 1 == grid.chunkSize) chunk.cells[x, y].neighbours[0] = chunk.neighbours[0].cells[grid.chunkSize-1, 0];
                        if (chunk.neighbours[2] is { } && x + 1 == grid.chunkSize && y + 1 == grid.chunkSize) chunk.cells[x, y].neighbours[2] = chunk.neighbours[2].cells[0, 0];
                        if (chunk.neighbours[5] is { } && x - 1 == -1 && y - 1 == -1) chunk.cells[x, y].neighbours[5] = chunk.neighbours[5].cells[grid.chunkSize - 1, grid.chunkSize - 1];
                        if (chunk.neighbours[7] is { } && x + 1 == grid.chunkSize && y - 1 == -1) chunk.cells[x, y].neighbours[7] = chunk.neighbours[7].cells[0, grid.chunkSize - 1];
                        
                        //! top
                        if (chunk.neighbours[1] is { } && y + 1 == grid.chunkSize)
                        {
                            chunk.cells[x, y].neighbours[1] = chunk.neighbours[1].cells[x, 0];
                            if (x > 0) chunk.cells[x, y].neighbours[0] = chunk.neighbours[1].cells[x-1, 0];
                            if (x < grid.chunkSize-1) chunk.cells[x, y].neighbours[2] = chunk.neighbours[1].cells[x+1, 0];
                        }

                        //! left
                        if (chunk.neighbours[3] is { } && x - 1 == -1)
                        {
                            chunk.cells[x, y].neighbours[3] = chunk.neighbours[3].cells[grid.chunkSize - 1, y];
                            if (y > 0) chunk.cells[x, y].neighbours[5] = chunk.neighbours[3].cells[grid.chunkSize-1, y-1];
                            if (y < grid.chunkSize-1) chunk.cells[x, y].neighbours[0] = chunk.neighbours[3].cells[grid.chunkSize-1, y+1];
                        }
                        
                        //! right
                        if (chunk.neighbours[4] is { } && x + 1 == grid.chunkSize)
                        {
                            chunk.cells[x, y].neighbours[4] = chunk.neighbours[4].cells[0, y];
                            if (y > 0) chunk.cells[x, y].neighbours[7] = chunk.neighbours[4].cells[0, y - 1];
                            if (y < grid.chunkSize - 1) chunk.cells[x, y].neighbours[2] = chunk.neighbours[4].cells[0, y+1];
                        }
                        
                        //! bottom
                        if (chunk.neighbours[6] is { } && y - 1 == -1)
                        {
                            chunk.cells[x, y].neighbours[6] = chunk.neighbours[6].cells[x, grid.chunkSize - 1];
                            if (x < grid.chunkSize - 1) chunk.cells[x, y].neighbours[7] = chunk.neighbours[6].cells[x+1, grid.chunkSize-1];
                            if (x > 0) chunk.cells[x, y].neighbours[5] = chunk.neighbours[6].cells[x-1, grid.chunkSize-1];
                        }
                    }
                }
            });
        }

        //> GENERATE CHUNKS
        public void GenerateAllChunks() => GenerateChunks(grid.chunks);
        private void GenerateChunks(Chunk[,] chunks)
        {
            // multi-threaded chunk generation
            var tasks = new List<Task>();
            chunks.ForEach(c => tasks.Add(Task.Factory.StartNew(() => GenerateCells(c.cells))));
            Task.WaitAll(tasks.ToArray());
            
            // apply triangulations on main thread
            // chunks.ForEach(c => UpdateTerrainTiles(0, c.cells));
        }

        //> GENERATE CELLS
        private void GenerateCells(Cell[,] cells) => cells.ForEach(c =>
        {
            var noiseValue = Noise.GenerateValue(grid.terrainNoise, c.position);
            // grid.noiseRange.Add(noiseValue);
            c.terrainValue = noiseValue;
        });

        public void GenerateAllBiomes() => grid.chunks.ForEach(c => GenerateBiomes(c.cells));
        private void GenerateBiomes(Cell[,] cells)
        {
            // var rainRange = new Range();
            // var heatRange = new Range();
            cells.ForEach(c =>
            {
                var rainValue = Noise.GenerateValue(grid.biomeNoise[0], c.position);
                // rainRange.Add(rainValue);
                var heatValue = Noise.GenerateValue(grid.biomeNoise[1], c.position);
                // heatRange.Add(heatValue);
                
                if (heatValue < 0.25f && rainValue > 0.25f) c.biome = Biome.Snow;
                if (heatValue < 0.50f && rainValue < 0.50f) c.biome = Biome.Stone;
                if (heatValue > 0.60f && rainValue < 0.25f) c.biome = Biome.Sand;
                if (heatValue < 0.60f && heatValue > 0.25f && rainValue < 0.25f) c.biome = Biome.Plains;
                if (heatValue > 0.25f && rainValue > 0.25f && rainValue < 0.75f) c.biome = Biome.Grass;
                if (heatValue > 0.50f && rainValue > 0.25f) c.biome = Biome.Forest;
                
                if (c.terrainValue < grid.terrainNoise[0].threshold) c.biome = Biome.Ocean;
                // if (c.terrainValue < grid.terrainNoise[0].threshold) c.biome = Biome.Ocean;
                // else c.biome = Biome.Stone;
            });

            // Debug.Log($"RAIN: {rainRange} | HEAT: {heatRange}");

            UpdateTerrainTiles(0, cells);
        }

        //> GENERATE RESOURCES
        // public void GenerateAllResources() => grid.chunks.ForEach(c => GenerateResources(c.cells));
        public void GenerateAllResources() { foreach (var c in grid.chunks) GenerateResources(c.cells);}
        public void GenerateResources(Cell[,] cells)
        {
            foreach (var cell in cells)
            {
                cell.resourceDeposits.Clear();
                
                grid.resourceNoise.ForEach(rnl =>
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
                        Resource.Material.Iron     => grid.tiles[4],
                        Resource.Material.Gold     => grid.tiles[6],
                        Resource.Material.Coal     => grid.tiles[8],
                        Resource.Material.Copper   => grid.tiles[2],
                        Resource.Material.Platinum => grid.tiles[7],
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
                if (cell.biome == Biome.Ocean) cell.buildable = false;

                var tile = (cell.biome) switch
                {
                    // Biome.Forest => ,
                    Biome.Sand => grid.tiles[9],
                    Biome.Grass => grid.tiles[5],
                    Biome.Stone => grid.tiles[1],
                    Biome.Forest => grid.tiles[10],
                    Biome.Snow => grid.tiles[11],
                    Biome.Plains => grid.tiles[12],
                    Biome.Ocean => grid.tiles[0],
                    _ => grid.tiles[3],
                };
                // var tile = (cell.terrainValue >= grid.terrainNoise[0].threshold) ? grid.tiles[1] : grid.tiles[0];
                grid.tilemaps[map].SetTile(new Vector3Int(cell.coords.x, cell.coords.y, 0), tile);
            }
        }
        

        //> DRAW HELPFUL GIZMOS
        private void OnDrawGizmos()
        {
            if (!showNeighbours || grid.lastCell is null) return;
         
            for (int i = 0; i < grid.lastCell.neighbours.Length; i++)
            {
                if (grid.lastCell.neighbours[i] is null) continue;
        
                Gizmos.color = Color.Lerp(Color.red, Color.black, i / 7f);
                Gizmos.DrawSphere(grid.lastCell.neighbours[i].position, 0.25f);
            }
            
            for (int i = 0; i < grid.lastCell.parentChunk.neighbours.Length; i++)
            {
                if (grid.lastCell.parentChunk.neighbours[i] is null) continue;
                
                Gizmos.color = Color.Lerp(Color.green, Color.black, i / 7f);
                var chunkOffset = grid.lastCell.parentChunk.neighbours[i].chunkOffset.ToVector3();
                chunkOffset.x += grid.chunkSize / 2f;
                chunkOffset.y += grid.chunkSize / 2f;
                Gizmos.DrawSphere(chunkOffset, grid.chunkSize * 0.5f);
            }
        }
    }

    [Serializable] public class ResourceNoiseLayer : Noise.Layer
    {
        public Resource resource;
    }
}
