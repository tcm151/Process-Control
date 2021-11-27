using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using ProcessControl.Tools;
using ProcessControl.Industry;
#pragma warning disable 108, 114


namespace ProcessControl.Procedural
{
    [RequireComponent(typeof(Grid))]
    public class CellGrid : MonoBehaviour
    {
        [System.Serializable]
        public class Data
        {
            [Header("Resolution")]
            public int chunkSize = 64;
            public int size = 4;
            
            public List<Tilemap> tilemaps;
            public List<TileBase> tiles;
            
            [Header("Generation")]
            public string seed;
            public Range noiseRange;
            public List<Noise.Layer> terrainNoise;
            public List<ResourceNoiseLayer> resourceNoise;
            public Resource sandResource, stoneResource;
            public List<Noise.Layer> biomeNoise;

            [Header("Cells & Chunks")]
            public Chunk[,] chunks;
            public Cell lastCell;

            [Space]
            public bool showNeighbours;
        }
        [SerializeField] internal Data grid;

        
        public int renderDistance = 128;
        public Range rainRange = new Range();
        public Range heatRange = new Range();
        
        private Camera camera;
        private readonly System.Diagnostics.Stopwatch timer = new System.Diagnostics.Stopwatch();

        //> DECOUPLE EXTERNAL FUNCTION CALLS
        public static System.Func<Cell> GetCellUnderMouse;
        public static System.Func<Vector3, Cell> GetCellAtPosition;
        public static System.Func<Vector2Int, Cell> GetCellAtCoordinates;
        
        //> INITIALIZATION
        public void Awake()
        {
            // initialize
            timer.Start();
            CreateGrid();
            float init = timer.ElapsedMilliseconds;
            timer.Restart();
            
            // get closest chunks to spawn
            var closeChunks = grid.chunks.Where(c => Vector3.Distance(Vector3.zero, c.chunkCenter) < renderDistance);
            
            // generate the terrain
            GenerateChunks(closeChunks);
            float chunkGen = timer.ElapsedMilliseconds;
            timer.Reset();
            
            Debug.Log($"Generated: {init} | {chunkGen} |= {init+chunkGen} ms");

            CellSpawner.CalculateSpawnLocation();
        }
        
        //> EVENTS

        //> CACHE LAST TOUCHED CELL
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                grid.lastCell = GetCellUnderMouse();
            }
        }

        //> GET THE CELL AT GIVEN COORDS
        private Cell GetCellFromCoords(Vector2Int coords)
        {
            var cells = GetChunkFromCoords(coords).cells.ToList();
            return cells.FirstOrDefault(cell => cell.coords == coords);
        }

        //> GET THE CORRESPONDING CHUNK AT THE GIVEN COORDS
        private Chunk GetChunkFromCoords(Vector2Int coords)
        {
            var floatCoords = coords.ToVector2();
            float bounds = (grid.size * grid.chunkSize) / 2f;
            var x = floatCoords.x.Remap(-bounds, bounds - 1, 0, grid.size - 0.0001f).FloorToInt();
            var y = floatCoords.y.Remap(-bounds, bounds - 1, 0, grid.size - 0.0001f).FloorToInt();
            Chunk chunk = grid.chunks?[x,y];
            return chunk;
        }

        //> INITIALIZE THE GRID
        public void CreateGrid()
        {
            camera = Camera.main;

            //- register events
            GetCellAtCoordinates += GetCellFromCoords;
            GetCellUnderMouse += () => GetCellFromCoords(camera.MousePosition2D().FloorToInt());
            GetCellAtPosition += (position) => GetCellFromCoords(position.ToVector2().FloorToInt());

            //- random seed noise layers
            if (grid.seed != "") Random.InitState(grid.seed.GetHashCode());
            grid.biomeNoise.ForEach(b => b.offset = Random.insideUnitSphere * (Random.value * 5));
            grid.terrainNoise.ForEach(t => t.offset = Random.insideUnitSphere * (Random.value * 10));
            grid.resourceNoise.ForEach(r => r.offset = Random.insideUnitSphere * (Random.value * 2));

            //- gather and clear tilemaps
            grid.tilemaps = GetComponentsInChildren<Tilemap>().ToList();
            ClearAllTiles();

            
            //- create chunk array
            grid.chunks = new Chunk[grid.size, grid.size];
            for (int y = 0; y < grid.size; y++) {
                for (int x = 0; x < grid.size; x++)
                {
                    float xOffset = (x - (grid.size / 2f)) * grid.chunkSize;
                    float yOffset = (y - (grid.size / 2f)) * grid.chunkSize;
                    grid.chunks[x, y] = new Chunk
                    {
                        chunkCenter = new Vector3
                        {
                            x = xOffset + grid.chunkSize / 2f,
                            y = yOffset + grid.chunkSize / 2f,
                            z = 0f,
                        },
                        
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
                    //~ top left, top middle, top right
                    if (x - 1 >= 0 && y + 1 < grid.size)        grid.chunks[x, y].neighbours[0] = grid.chunks[x - 1, y + 1];
                    if (y + 1 < grid.size)                      grid.chunks[x, y].neighbours[1] = grid.chunks[  x  , y + 1];
                    if (x + 1 < grid.size && y + 1 < grid.size) grid.chunks[x, y].neighbours[2] = grid.chunks[x + 1, y + 1];

                    //~ left and right
                    if (x - 1 >= 0)        grid.chunks[x, y].neighbours[3] = grid.chunks[x - 1, y];
                    if (x + 1 < grid.size) grid.chunks[x, y].neighbours[4] = grid.chunks[x + 1, y];

                    //~ bottom left, bottom middle, bottomRight
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
                        };
                    }
                }
            });
            
            //- assign cell neighbours
            grid.chunks.ForEach(chunk =>
            {
                // loop over every cell in said chunk
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

                        //$ corners
                        if (chunk.neighbours[0] is { } && x - 1 == -1 && y + 1 == grid.chunkSize) chunk.cells[x, y].neighbours[0] = chunk.neighbours[0].cells[grid.chunkSize-1, 0];
                        if (chunk.neighbours[2] is { } && x + 1 == grid.chunkSize && y + 1 == grid.chunkSize) chunk.cells[x, y].neighbours[2] = chunk.neighbours[2].cells[0, 0];
                        if (chunk.neighbours[5] is { } && x - 1 == -1 && y - 1 == -1) chunk.cells[x, y].neighbours[5] = chunk.neighbours[5].cells[grid.chunkSize - 1, grid.chunkSize - 1];
                        if (chunk.neighbours[7] is { } && x + 1 == grid.chunkSize && y - 1 == -1) chunk.cells[x, y].neighbours[7] = chunk.neighbours[7].cells[0, grid.chunkSize - 1];
                        
                        //$ top
                        if (chunk.neighbours[1] is { } && y + 1 == grid.chunkSize)
                        {
                            chunk.cells[x, y].neighbours[1] = chunk.neighbours[1].cells[x, 0];
                            if (x > 0) chunk.cells[x, y].neighbours[0] = chunk.neighbours[1].cells[x-1, 0];
                            if (x < grid.chunkSize-1) chunk.cells[x, y].neighbours[2] = chunk.neighbours[1].cells[x+1, 0];
                        }

                        //$ left
                        if (chunk.neighbours[3] is { } && x - 1 == -1)
                        {
                            chunk.cells[x, y].neighbours[3] = chunk.neighbours[3].cells[grid.chunkSize - 1, y];
                            if (y > 0) chunk.cells[x, y].neighbours[5] = chunk.neighbours[3].cells[grid.chunkSize-1, y-1];
                            if (y < grid.chunkSize-1) chunk.cells[x, y].neighbours[0] = chunk.neighbours[3].cells[grid.chunkSize-1, y+1];
                        }
                        
                        //$ right
                        if (chunk.neighbours[4] is { } && x + 1 == grid.chunkSize)
                        {
                            chunk.cells[x, y].neighbours[4] = chunk.neighbours[4].cells[0, y];
                            if (y > 0) chunk.cells[x, y].neighbours[7] = chunk.neighbours[4].cells[0, y - 1];
                            if (y < grid.chunkSize - 1) chunk.cells[x, y].neighbours[2] = chunk.neighbours[4].cells[0, y+1];
                        }
                        
                        //$ bottom
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
        public void GenerateAllChunks() => GenerateChunks(grid.chunks.ToList());
        private void GenerateChunks(List<Chunk> chunks)
        {
            // multi-threaded chunk generation
            var tasks = new List<Task>();
            chunks.ForEach(chunk =>
            {
                tasks.Add(Task.Run(() =>
                {
                    GenerateTerrain(chunk);
                    GenerateResources(chunk);
                }));
            });
            Task.WaitAll(tasks.ToArray());


            // update tile maps on main thread
            chunks.ForEach(UpdateTileMaps);
        }

        //> GENERATE TERRAIN
        public void GenerateAllTerrain() => grid.chunks.ForEach(GenerateTerrain);
        private void GenerateTerrain(Chunk chunk) => chunk.cells.ForEach(cell =>
        {
            var noiseValue = Noise.GenerateValue(grid.terrainNoise, cell.position);
            grid.noiseRange.Add(noiseValue);
            cell.terrainValue = noiseValue;
                
            var rainValue = Noise.GenerateValue(grid.biomeNoise[0], cell.position);
            // rainRange.Add(rainValue);
            var heatValue = Noise.GenerateValue(grid.biomeNoise[1], cell.position);
            // heatRange.Add(heatValue);
                
            if (heatValue < 0.25f && rainValue > 0.25f) cell.biome = Biome.Snow;
            if (heatValue < 0.50f && rainValue < 0.50f) cell.biome = Biome.Stone;
            if (heatValue > 0.60f && rainValue < 0.25f) cell.biome = Biome.Sand;
            if (heatValue < 0.60f && heatValue > 0.25f && rainValue < 0.25f) cell.biome = Biome.Plains;
            if (heatValue > 0.25f && rainValue > 0.25f && rainValue < 0.75f) cell.biome = Biome.Grass;
            if (heatValue > 0.50f && rainValue > 0.25f) cell.biome = Biome.Forest;
                
            if (cell.terrainValue < grid.terrainNoise[0].threshold) cell.biome = Biome.Ocean;
            // if (c.terrainValue < grid.terrainNoise[0].threshold) c.biome = Biome.Ocean;
            // else c.biome = Biome.Stone;
        });

        //> GENERATE RESOURCES
        public void GenerateAllResources() => grid.chunks.ForEach(GenerateResources);
        public void GenerateResources(Chunk chunk) => chunk.cells.ForEach(cell =>
        {
            cell.resourceDeposits.Clear();
                
            grid.resourceNoise.ForEach(resourceLayer =>
            {
                var noiseValue = Noise.GenerateValue(resourceLayer, cell.position);
                if (noiseValue >= resourceLayer.threshold && cell.buildable)
                {
                    cell.resourceDeposits.Add(new ResourceDeposit
                    {
                        noiseValue = noiseValue,
                        quantity = (noiseValue * 16484f).FloorToInt(),
                        resource = resourceLayer.resource,
                    });
                }
            });

            switch (cell.biome)
            {
                case Biome.Sand: cell.resourceDeposits.Add(new ResourceDeposit
                {
                    resource = grid.sandResource,
                    quantity = 10000,
                }); break;
                    
                case Biome.Stone: cell.resourceDeposits.Add(new ResourceDeposit
                {
                    resource = grid.stoneResource,
                    quantity = 10000,
                }); break;
            }

            if (cell.resourceDeposits.Count == 0)
            {
                cell.resourceDeposits.Add(new ResourceDeposit
                {
                    resource = grid.stoneResource,
                    quantity = 10000,
                });
            }
        });

        //> TILE MODIFICATION
        public void ClearAllTiles() => grid.tilemaps.ForEach(t => t.ClearAllTiles());
        public void UpdateTileMaps(Chunk chunk) => chunk.cells.ForEach(cell =>
        {
            if (cell.biome == Biome.Ocean) cell.buildable = false;

            var tile = (cell.biome) switch
            {
                Biome.Sand   => grid.tiles[9],
                Biome.Grass  => grid.tiles[5],
                Biome.Stone  => grid.tiles[1],
                Biome.Forest => grid.tiles[10],
                Biome.Snow   => grid.tiles[11],
                Biome.Plains => grid.tiles[12],
                Biome.Ocean  => grid.tiles[0],
                _            => grid.tiles[3],
            };
            grid.tilemaps[0].SetTile(new Vector3Int(cell.coords.x, cell.coords.y, 0), tile);
            
            if (cell.resourceDeposits.Count == 0) tile = grid.tiles[3];
            else
            {
                tile = (cell.resourceDeposits[0].resource.material) switch
                {
                    Resource.Material.Iron     => grid.tiles[4],
                    Resource.Material.Gold     => grid.tiles[6],
                    Resource.Material.Coal     => grid.tiles[8],
                    Resource.Material.Copper   => grid.tiles[2],
                    Resource.Material.Platinum => grid.tiles[7],
                    _                          => grid.tiles[3],
                };
            }
            grid.tilemaps[1].SetTile(new Vector3Int(cell.coords.x, cell.coords.y, 0), tile);
        });

        //> DRAW HELPFUL GIZMOS
        private void OnDrawGizmos()
        {
            
            if (!grid.showNeighbours || grid.lastCell is null) return;
         
            grid.chunks?.ForEach(
                c =>
                {
                    if (c is null) return;
                    Gizmos.DrawSphere(c.chunkCenter, 1f);
                });
            
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
}
