using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;
using ProcessControl.Tools;
using ProcessControl.Industry;
using UnityEngine.Serialization;
using Range = ProcessControl.Tools.Range;

#pragma warning disable 108, 114


namespace ProcessControl.Procedural
{
    [Serializable]
    public class CellGridData : Data<CellGrid>
    {
        [Header("Resolution")]
        public int chunkSize = 64;
        public int size = 4;
        public int renderDistance = 128;
            
            
        [Header("Generation")]
        public string seed;
        public Range noiseRange = new Range();
        public Range rainRange = new Range();
        public Range heatRange = new Range();
        public List<Noise.Layer> terrainNoise;
        public List<ResourceNoiseLayer> resourceNoise;
        public List<Noise.Layer> biomeNoise;

        [Header("Cells & Chunks")]
        public Chunk[,] chunks;
        public Cell lastCell;

        [Space]
        public bool showNeighbours;

        public void Save()
        {
            FileManager.WriteFile($"{seed}.json", this);
        }

        public void Load()
        {
            var data = FileManager.ReadFile<CellGridData>($"{seed}.json");
            chunkSize = data.chunkSize;
            size = data.size;
            renderDistance = data.renderDistance;
            seed = data.seed;
            terrainNoise = data.terrainNoise;
            resourceNoise = data.resourceNoise;
            biomeNoise = data.biomeNoise;
        }
    }
    
    [RequireComponent(typeof(Grid))]
    public class CellGrid : MonoBehaviour
    {
        [SerializeReference] public List<TileBase> tiles;
        [SerializeReference] public List<Tilemap> tilemaps;
        
        [FormerlySerializedAs("gridData")][SerializeField] internal CellGridData gridCellGridData;

        private bool GridExists => gridCellGridData.chunks is { };

        public static event Action onFinishWorldGeneration;
        public static event Action onStartWorldGeneration;
        
        private Camera camera;
        private readonly System.Diagnostics.Stopwatch timer = new System.Diagnostics.Stopwatch();

        //> DECOUPLE EXTERNAL FUNCTION CALLS
        public static Func<Cell> GetCellUnderMouse;
        public static Func<Vector3, Cell> GetCellAtPosition;
        public static Func<Vector2Int, Cell> GetCellAtCoordinates;

        //> CACHE DATA AND REGISTER EVENTS
        private void Awake()
        {
            camera = Camera.main;

            //- register events
            GetCellAtCoordinates += GetCellFromCoords;
            GetCellUnderMouse += () => GetCellFromCoords(camera.MousePosition2D().FloorToInt());
            GetCellAtPosition += (position) => GetCellFromCoords(position.ToVector2().FloorToInt());
        }
        
        //> INITIALIZATION
        public async void Start()
        {
            onStartWorldGeneration?.Invoke();
            await Task.Yield();
            
            Initialize();   
        }

        public async void Initialize()
        {
            timer.Start();
            if (!GridExists) CreateGrid();
            float init = timer.ElapsedMilliseconds;
            
            // get closest chunks to spawn
            var closeChunks = gridCellGridData.chunks.Where(c => Vector3.Distance(Vector3.zero, c.chunkCenter) < gridCellGridData.renderDistance);
            
            // generate the terrain
            timer.Restart();
            await GenerateChunks(closeChunks);
            float chunkGen = timer.ElapsedMilliseconds;
            timer.Stop();
            timer.Reset();
            
            Debug.Log($"Generated: {init} | {chunkGen} |= {init+chunkGen} ms");

            if (Application.isPlaying) CellSpawner.FindSpawnLocation();
            onFinishWorldGeneration?.Invoke();
        }
        
        //> CACHE LAST TOUCHED CELL
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                gridCellGridData.lastCell = GetCellUnderMouse();
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
            float bounds = (gridCellGridData.size * gridCellGridData.chunkSize) / 2f;
            var x = floatCoords.x.Remap(-bounds, bounds - 1, 0, gridCellGridData.size - 0.0001f).FloorToInt();
            var y = floatCoords.y.Remap(-bounds, bounds - 1, 0, gridCellGridData.size - 0.0001f).FloorToInt();
            Chunk chunk = gridCellGridData.chunks?[x,y];
            return chunk;
        }

        //> INITIALIZE THE GRID
        public void CreateGrid()
        {
            //- random seed noise layers
            if (gridCellGridData.seed != "") Random.InitState(gridCellGridData.seed.GetHashCode());
            gridCellGridData.biomeNoise.ForEach(b => b.offset = Random.insideUnitSphere * (Random.value * 5));
            gridCellGridData.terrainNoise.ForEach(t => t.offset = Random.insideUnitSphere * (Random.value * 10));
            gridCellGridData.resourceNoise.ForEach(r => r.offset = Random.insideUnitSphere * (Random.value * 2));

            //- gather and clear tilemaps
            tilemaps = GetComponentsInChildren<Tilemap>().ToList();
            ClearAllTiles();
            
            //- create chunk array
            gridCellGridData.chunks = new Chunk[gridCellGridData.size, gridCellGridData.size];
            for (int y = 0; y < gridCellGridData.size; y++) {
                for (int x = 0; x < gridCellGridData.size; x++)
                {
                    float xOffset = (x - (gridCellGridData.size / 2f)) * gridCellGridData.chunkSize;
                    float yOffset = (y - (gridCellGridData.size / 2f)) * gridCellGridData.chunkSize;
                    gridCellGridData.chunks[x, y] = new Chunk
                    {
                        chunkCenter = new Vector3
                        {
                            x = xOffset + gridCellGridData.chunkSize / 2f,
                            y = yOffset + gridCellGridData.chunkSize / 2f,
                            z = 0f,
                        },
                        
                        chunkOffset = new Vector2Int(xOffset.FloorToInt(), yOffset.FloorToInt()),
                        neighbours = new Chunk[8],
                        cells = new Cell[gridCellGridData.chunkSize, gridCellGridData.chunkSize],
                    };
                }
            }
            
            //- assign chunk neighbours
            for (int y = 0; y < gridCellGridData.size; y++) {
                for (int x = 0; x < gridCellGridData.size; x++)
                {
                    //~ top left, top middle, top right
                    if (x - 1 >= 0 && y + 1 < gridCellGridData.size)        gridCellGridData.chunks[x, y].neighbours[0] = gridCellGridData.chunks[x - 1, y + 1];
                    if (y + 1 < gridCellGridData.size)                      gridCellGridData.chunks[x, y].neighbours[1] = gridCellGridData.chunks[  x  , y + 1];
                    if (x + 1 < gridCellGridData.size && y + 1 < gridCellGridData.size) gridCellGridData.chunks[x, y].neighbours[2] = gridCellGridData.chunks[x + 1, y + 1];

                    //~ left and right
                    if (x - 1 >= 0)        gridCellGridData.chunks[x, y].neighbours[3] = gridCellGridData.chunks[x - 1, y];
                    if (x + 1 < gridCellGridData.size) gridCellGridData.chunks[x, y].neighbours[4] = gridCellGridData.chunks[x + 1, y];

                    //~ bottom left, bottom middle, bottomRight
                    if (x - 1 >= 0 && y - 1 >= 0)        gridCellGridData.chunks[x, y].neighbours[5] = gridCellGridData.chunks[x - 1, y - 1];
                    if (y - 1 >= 0)                      gridCellGridData.chunks[x, y].neighbours[6] = gridCellGridData.chunks[  x  , y - 1];
                    if (x + 1 < gridCellGridData.size && y - 1 >= 0) gridCellGridData.chunks[x, y].neighbours[7] = gridCellGridData.chunks[x + 1, y - 1];
                }
            }
            
            //- create cell arrays
            gridCellGridData.chunks.ForEach(chunk =>
            {
                for (int y = 0; y < gridCellGridData.chunkSize; y++) {
                    for (int x = 0; x < gridCellGridData.chunkSize; x++)
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
            gridCellGridData.chunks.ForEach(chunk =>
            {
                // loop over every cell in said chunk
                for (int y = 0; y < gridCellGridData.chunkSize; y++) {
                    for (int x = 0; x < gridCellGridData.chunkSize; x++)
                    {
                        if (x - 1 >= 0 && y + 1 < gridCellGridData.chunkSize) chunk.cells[x, y].neighbours[0] = chunk.cells[x - 1, y + 1];
                        if (y + 1 < gridCellGridData.chunkSize) chunk.cells[x, y].neighbours[1] = chunk.cells[x, y + 1];
                        if (x + 1 < gridCellGridData.chunkSize && y + 1 < gridCellGridData.chunkSize) chunk.cells[x, y].neighbours[2] = chunk.cells[x + 1, y + 1];

                        if (x - 1 >= 0) chunk.cells[x, y].neighbours[3] = chunk.cells[x - 1, y];
                        if (x + 1 < gridCellGridData.chunkSize) chunk.cells[x, y].neighbours[4] = chunk.cells[x + 1, y];

                        if (x - 1 >= 0 && y - 1 >= 0) chunk.cells[x, y].neighbours[5] = chunk.cells[x - 1, y - 1];
                        if (y - 1 >= 0) chunk.cells[x, y].neighbours[6] = chunk.cells[x, y - 1];
                        if (x + 1 < gridCellGridData.chunkSize && y - 1 >= 0) chunk.cells[x, y].neighbours[7] = chunk.cells[x + 1, y - 1];

                        //$ corners
                        if (chunk.neighbours[0] is { } && x - 1 == -1 && y + 1 == gridCellGridData.chunkSize) chunk.cells[x, y].neighbours[0] = chunk.neighbours[0].cells[gridCellGridData.chunkSize-1, 0];
                        if (chunk.neighbours[2] is { } && x + 1 == gridCellGridData.chunkSize && y + 1 == gridCellGridData.chunkSize) chunk.cells[x, y].neighbours[2] = chunk.neighbours[2].cells[0, 0];
                        if (chunk.neighbours[5] is { } && x - 1 == -1 && y - 1 == -1) chunk.cells[x, y].neighbours[5] = chunk.neighbours[5].cells[gridCellGridData.chunkSize - 1, gridCellGridData.chunkSize - 1];
                        if (chunk.neighbours[7] is { } && x + 1 == gridCellGridData.chunkSize && y - 1 == -1) chunk.cells[x, y].neighbours[7] = chunk.neighbours[7].cells[0, gridCellGridData.chunkSize - 1];
                        
                        //$ top
                        if (chunk.neighbours[1] is { } && y + 1 == gridCellGridData.chunkSize)
                        {
                            chunk.cells[x, y].neighbours[1] = chunk.neighbours[1].cells[x, 0];
                            if (x > 0) chunk.cells[x, y].neighbours[0] = chunk.neighbours[1].cells[x-1, 0];
                            if (x < gridCellGridData.chunkSize-1) chunk.cells[x, y].neighbours[2] = chunk.neighbours[1].cells[x+1, 0];
                        }

                        //$ left
                        if (chunk.neighbours[3] is { } && x - 1 == -1)
                        {
                            chunk.cells[x, y].neighbours[3] = chunk.neighbours[3].cells[gridCellGridData.chunkSize - 1, y];
                            if (y > 0) chunk.cells[x, y].neighbours[5] = chunk.neighbours[3].cells[gridCellGridData.chunkSize-1, y-1];
                            if (y < gridCellGridData.chunkSize-1) chunk.cells[x, y].neighbours[0] = chunk.neighbours[3].cells[gridCellGridData.chunkSize-1, y+1];
                        }
                        
                        //$ right
                        if (chunk.neighbours[4] is { } && x + 1 == gridCellGridData.chunkSize)
                        {
                            chunk.cells[x, y].neighbours[4] = chunk.neighbours[4].cells[0, y];
                            if (y > 0) chunk.cells[x, y].neighbours[7] = chunk.neighbours[4].cells[0, y - 1];
                            if (y < gridCellGridData.chunkSize - 1) chunk.cells[x, y].neighbours[2] = chunk.neighbours[4].cells[0, y+1];
                        }
                        
                        //$ bottom
                        if (chunk.neighbours[6] is { } && y - 1 == -1)
                        {
                            chunk.cells[x, y].neighbours[6] = chunk.neighbours[6].cells[x, gridCellGridData.chunkSize - 1];
                            if (x < gridCellGridData.chunkSize - 1) chunk.cells[x, y].neighbours[7] = chunk.neighbours[6].cells[x+1, gridCellGridData.chunkSize-1];
                            if (x > 0) chunk.cells[x, y].neighbours[5] = chunk.neighbours[6].cells[x-1, gridCellGridData.chunkSize-1];
                        }
                    }
                }
            });
            
        }

        //> GENERATE CHUNKS
        // public async void GenerateAllChunks() => await GenerateChunks(grid.chunks.ToList());
        private async Task GenerateChunks(List<Chunk> chunks)
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
            await Task.WhenAll(tasks.ToArray());
            
            // update tile maps on main thread
            chunks.ForEach(UpdateTileMaps);
        }

        //> GENERATE TERRAIN
        // public void GenerateAllTerrain() => grid.chunks.ForEach(GenerateTerrain);
        private async void GenerateTerrain(Chunk chunk)
        {
            // multi-threaded chunk generation
            var tasks = new List<Task>();
            chunk.cells.ForEach(cell =>
            {
                tasks.Add(Task.Run(() =>
                {
                    var noiseValue = Noise.GenerateValue(gridCellGridData.terrainNoise, cell.position);
                    var rainValue = Noise.GenerateValue(gridCellGridData.biomeNoise[0], cell.position);
                    var heatValue = Noise.GenerateValue(gridCellGridData.biomeNoise[1], cell.position);
                    gridCellGridData.noiseRange.Add(noiseValue);
                    gridCellGridData.rainRange.Add(rainValue);
                    gridCellGridData.heatRange.Add(heatValue);
                    cell.terrainValue = noiseValue;


                    if (heatValue < 0.25f && rainValue > 0.25f) cell.biome = Biome.Snow;
                    if (heatValue < 0.50f && rainValue < 0.50f) cell.biome = Biome.Stone;
                    if (heatValue > 0.60f && rainValue < 0.25f) cell.biome = Biome.Sand;
                    if (heatValue < 0.60f && heatValue > 0.25f && rainValue < 0.25f) cell.biome = Biome.Plains;
                    if (heatValue > 0.25f && rainValue > 0.25f && rainValue < 0.75f) cell.biome = Biome.Grass;
                    if (heatValue > 0.50f && rainValue > 0.25f) cell.biome = Biome.Forest;

                    if (cell.terrainValue < gridCellGridData.terrainNoise[0].threshold)
                    {
                        cell.biome = Biome.Ocean;
                        cell.buildable = false;
                    }
                }));
            });
            await Task.WhenAll(tasks.ToArray());
            
            // chunk.cells.ForEach
            // (
            //     cell =>
            //     {
            //         var noiseValue = Noise.GenerateValue(grid.terrainNoise, cell.position);
            //         var rainValue = Noise.GenerateValue(grid.biomeNoise[0], cell.position);
            //         var heatValue = Noise.GenerateValue(grid.biomeNoise[1], cell.position);
            //         grid.noiseRange.Add(noiseValue);
            //         grid.rainRange.Add(rainValue);
            //         grid.heatRange.Add(heatValue);
            //         cell.terrainValue = noiseValue;
            //
            //
            //         if (heatValue < 0.25f && rainValue > 0.25f) cell.biome = Biome.Snow;
            //         if (heatValue < 0.50f && rainValue < 0.50f) cell.biome = Biome.Stone;
            //         if (heatValue > 0.60f && rainValue < 0.25f) cell.biome = Biome.Sand;
            //         if (heatValue < 0.60f && heatValue > 0.25f && rainValue < 0.25f) cell.biome = Biome.Plains;
            //         if (heatValue > 0.25f && rainValue > 0.25f && rainValue < 0.75f) cell.biome = Biome.Grass;
            //         if (heatValue > 0.50f && rainValue > 0.25f) cell.biome = Biome.Forest;
            //
            //         if (cell.terrainValue < grid.terrainNoise[0].threshold)
            //         {
            //             cell.biome = Biome.Ocean;
            //             cell.buildable = false;
            //         }
            //     }
            // );
        }

        //> GENERATE RESOURCES
        // public void GenerateAllResources() => grid.chunks.ForEach(GenerateResources);
        public async void GenerateResources(Chunk chunk)
        {
            // multi-threaded chunk generation
            var tasks = new List<Task>();
            chunk.cells.ForEach(cell =>
            {
                tasks.Add(Task.Run(() =>
                {
                    cell.resourceDeposits.Clear();

                    gridCellGridData.resourceNoise.ForEach(resourceLayer =>
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

                    var itemFactory = ServiceManager.Current.RequestService<ItemFactory>();
                    
                    switch (cell.biome)
                    {
                        case Biome.Sand:
                            cell.resourceDeposits.Add(new ResourceDeposit
                            {
                                resource = itemFactory.Get<Resource>("Sand"),
                                quantity = 10000,
                            });
                            break;

                        case Biome.Stone:
                            cell.resourceDeposits.Add(new ResourceDeposit
                            {
                                resource = itemFactory.Get<Resource>("Stone"),
                                quantity = 10000,
                            });
                            break;
                    }

                    if (cell.resourceDeposits.Count == 0)
                    {
                        cell.resourceDeposits.Add(new ResourceDeposit
                        {
                            resource = itemFactory.Get<Resource>("Stone"),
                            quantity = 10000,
                        });
                    }
                }));
            });
            await Task.WhenAll(tasks.ToArray());
            
        }

        //> TILE MODIFICATION
        public void ClearAllTiles() => tilemaps.ForEach(t => t.ClearAllTiles());
        public void UpdateTileMaps(Chunk chunk) => chunk.cells.ForEach(cell =>
        {
            var tile = (cell.biome) switch
            {
                Biome.Sand   => tiles[9],
                Biome.Grass  => tiles[5],
                Biome.Stone  => tiles[1],
                Biome.Forest => tiles[10],
                Biome.Snow   => tiles[11],
                Biome.Plains => tiles[12],
                Biome.Ocean  => tiles[0],
                _            => tiles[3],
            };
            tilemaps[0].SetTile(new Vector3Int(cell.coords.x, cell.coords.y, 0), tile);
            
            if (cell.resourceDeposits.Count == 0) tile = tiles[3];
            else
            {
                tile = (cell.resourceDeposits[0].resource.material) switch
                {
                    Resource.Material.Iron     => tiles[4],
                    Resource.Material.Gold     => tiles[6],
                    Resource.Material.Coal     => tiles[8],
                    Resource.Material.Copper   => tiles[2],
                    Resource.Material.Platinum => tiles[7],
                    _                          => tiles[3],
                };
            }
            tilemaps[1].SetTile(new Vector3Int(cell.coords.x, cell.coords.y, 0), tile);
        });

        //> DRAW HELPFUL GIZMOS
        private void OnDrawGizmos()
        {
            
            if (!gridCellGridData.showNeighbours || gridCellGridData.lastCell is null) return;
         
            gridCellGridData.chunks?.ForEach(
                c =>
                {
                    if (c is null) return;
                    Gizmos.DrawSphere(c.chunkCenter, 1f);
                });
            
            for (int i = 0; i < gridCellGridData.lastCell.neighbours.Length; i++)
            {
                if (gridCellGridData.lastCell.neighbours[i] is null) continue;
        
                Gizmos.color = Color.Lerp(Color.red, Color.black, i / 7f);
                Gizmos.DrawSphere(gridCellGridData.lastCell.neighbours[i].position, 0.25f);
            }
            
            for (int i = 0; i < gridCellGridData.lastCell.parentChunk.neighbours.Length; i++)
            {
                if (gridCellGridData.lastCell.parentChunk.neighbours[i] is null) continue;
                
                Gizmos.color = Color.Lerp(Color.green, Color.black, i / 7f);
                var chunkOffset = gridCellGridData.lastCell.parentChunk.neighbours[i].chunkOffset.ToVector3();
                chunkOffset.x += gridCellGridData.chunkSize / 2f;
                chunkOffset.y += gridCellGridData.chunkSize / 2f;
                Gizmos.DrawSphere(chunkOffset, gridCellGridData.chunkSize * 0.5f);
            }
        }
    }
}
