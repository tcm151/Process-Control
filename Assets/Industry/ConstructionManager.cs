using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using ProcessControl.Jobs;
using ProcessControl.Tools;
using ProcessControl.Graphs;
using ProcessControl.Procedural;
using ProcessControl.Pathfinding;
using UnityEngine.Serialization;

#pragma warning disable 108,114


namespace ProcessControl.Industry
{
    public class ConstructionManager : MonoBehaviour
    {
        //> EVENT TRIGGERS
        public static Action<Schematic> SetPart;
        public static Action<bool> SetEdgeMode;

        //> EVENT SUBSCRIPTIONS
        public static Action<bool> OnBuildModeChanged;

        public bool queueJobGlobal = true;
        [SerializeField] private bool buildMode;
        [SerializeField] private bool conveyorMode;
        [FormerlySerializedAs("selectedPart")][SerializeField] private Schematic selectedSchematic;
        
        private Camera camera;
        public Node firstNode, secondNode;
        public Cell firstCell, secondCell;
        // [SerializeField] private List<Machine> builtMachines = new List<Machine>();
        [SerializeField] private List<HasInventory> inventories = new List<HasInventory>();

        public Junction defaultJunction;
        public Recipe defaultJunctionRecipe;

        //> INITIALIZATION
        private void Awake()
        {
            camera = Camera.main;

            SetPart += (part) => selectedSchematic = part;
            SetEdgeMode += (edgeMode) => conveyorMode = edgeMode;

            CellSpawner.onStartLocationDetermined += GenerateSpawnArea;
        }
        
        private void GenerateSpawnArea(Vector2 startPosition)
        {
            var stone = ItemFactory.GetItem("Stone");
            for (int i = 0; i < 64; i++)
            {
                var spawnPosition = CellSpawner.GenerateRandomSpawn(c => c.buildable, startPosition.FloorToInt(), 100);
                ItemFactory.SpawnContainer(stone, spawnPosition);
            }
            
            var ironIngot = ItemFactory.GetItem("Iron Ingot");
            for (int i = 0; i < 32; i++)
            {
                var spawnPosition = CellSpawner.GenerateRandomSpawn(c => c.buildable, startPosition.FloorToInt(), 100);
                ItemFactory.SpawnContainer(ironIngot, spawnPosition);
            }
            
            var ironPlate = ItemFactory.GetItem("Iron Plate");
            for (int i = 0; i < 32; i++)
            {
                var spawnPosition = CellSpawner.GenerateRandomSpawn(c => c.buildable, startPosition.FloorToInt(), 100);
                ItemFactory.SpawnContainer(ironPlate, spawnPosition);
            }

            var coalOre = ItemFactory.GetItem("Coal Ore");
            for (int i = 0; i < 32; i++)
            {
                var spawnPosition = CellSpawner.GenerateRandomSpawn(c => c.buildable, startPosition.FloorToInt(), 100);
                ItemFactory.SpawnContainer(coalOre, spawnPosition);
            }

            var ironBeam = ItemFactory.GetItem("Iron Beam");
            for (int i = 0; i < 32; i++)
            {
                var spawnPosition = CellSpawner.GenerateRandomSpawn(c => c.buildable, startPosition.FloorToInt(), 100);
                ItemFactory.SpawnContainer(ironBeam, spawnPosition);
            }

            var ironGear = ItemFactory.GetItem("Iron Gear");
            for (int i = 0; i < 32; i++)
            {
                var spawnPosition = CellSpawner.GenerateRandomSpawn(c => c.buildable, startPosition.FloorToInt(), 100);
                ItemFactory.SpawnContainer(ironGear, spawnPosition);
            }
            
            var storage = ItemFactory.GetItem("Storage Container");
            if (storage is Schematic part)
            {
                for (int i = 0; i < 1; i++)
                {
                    var cell = CellGrid.GetCellAtPosition(CellSpawner.GenerateRandomSpawn(c => c.buildable, startPosition.FloorToInt(), 25));
                    var node = PlaceNode(part.entity as Node, part.recipe, cell, false);
                    if (node is Buildable buildable) buildable.Build(0);

                    if (node is HasInventory inventory)
                    {
                        inventories.Add(inventory);
                        
                        inventory.Deposit(new Stack
                        {
                            item = ironPlate,
                            amount = 128,
                        });
                        
                        inventory.Deposit(new Stack
                        {
                            item = coalOre,
                            amount = 128,
                        });
                        
                        inventory.Deposit(new Stack
                        {
                            item = ironBeam,
                            amount = 128,
                        });
                        
                        inventory.Deposit(new Stack
                        {
                            item = ironIngot,
                            amount = 128,
                        });
                        
                        inventory.Deposit(new Stack
                        {
                            item = stone,
                            amount = 128,
                        });
                        
                        inventory.Deposit(new Stack
                        {
                            item = ironGear,
                            amount = 128,
                        });
                        
                        inventory.Deposit(new Stack
                        {
                            item = ItemFactory.GetItem("Machine Frame"),
                            amount = 1,
                        });
                        
                        inventory.Deposit(new Stack
                        {
                            item = ItemFactory.GetItem("Conveyor x64"),
                            amount = 32,
                        });
                    }
                    
                }
            }
            
            // var machines = FindObjectsOfType<Machine>();
            // machines.ForEach(m => builtMachines.Add(m));
        }

        //> BUILD EDGE BETWEEN TWO NODES
        private void BuildEdgeBetween(Recipe conveyorRecipe, Node firstNode, Node secondNode)
        {
            if (!(selectedSchematic.entity is Edge selectedEdge)) return;
            
            bool cannotBuild = false;
            // var matchingEntities = new List<Entity>();
            // var requiredContainers = new List<Container>();
            var matchingInventories = new List<(HasInventory inventory, Stack itemAmount)>();
            if (queueJobGlobal)
            {
                conveyorRecipe.inputItems.ForEach(itemAmount =>
                {
                    // var matchingContainers = ItemFactory.FindItemsByClosest(selectedNode.position, i);
                    // if (matchingContainers.Count < i.amount)
                    // {
                    // Debug.Log("NOT ENOUGH FOR RECIPE ITEMS...");
                    // cannotBuild = true;
                    // return;
                    // }
                    
                    // matchingContainers.ForEach(c => requiredContainers.Add(c));
                    // var matchingMachines = builtMachines.Where(m => m.inputInventory.Contains(i) || m.outputInventory.Contains(i)).ToList();
                    // matchingMachines.ForEach(m => matchingInventories.Add(m));
                    // Debug.Log($"Matching machines length: {matchingMachines.Count}");

                    var bestMatch = inventories.Where(inv => inv.Contains(itemAmount))
                                               .OrderBy(inv => Vector3.Distance((firstNode.position + secondNode.position) / 2f, inv.position))
                                               .FirstOrDefault();

                    if (bestMatch is null) cannotBuild = true;
                   
                    matchingInventories.Add((bestMatch, itemAmount));
                });
            }

            if (cannotBuild)
            {
                Debug.Log($"Unable to build {selectedSchematic.entity.name}");
                return;
            }
            
            firstCell = firstNode.parentCell;
            secondCell = secondNode.parentCell;
                
            if ((firstCell.coords.x == secondCell.coords.x) == (firstCell.coords.y == secondCell.coords.y))
            {
                Debug.Log("Conveyors node not in straight line...");
                return;
            }

            var conveyor = Factory.Spawn("Conveyors", selectedEdge, (firstNode.position + secondNode.position) / 2f);
            conveyor.enabled = false;

            if (firstNode is IO firstIO && secondNode is IO secondIO && conveyor is IO conveyorIO)
            {
                firstIO.ConnectOutput(conveyorIO);
                conveyorIO.ConnectInput(firstIO);
                conveyorIO.ConnectOutput(secondIO);
                secondIO.ConnectInput(conveyorIO);
            }
            else
            {
                Debug.Log("NODES WERE NOT MACHINES...");
            }
            

            var conveyorPath = AStar.FindPath(firstCell.position, secondCell.position, true);
            conveyorPath.ForEach(p =>
            {
                var tile = CellGrid.GetCellAtPosition(p);
                tile.edges.Add(conveyor);
                if (conveyor is Conveyor c) c.tilesCovered.Add(tile);
            });

            if (conveyor is Buildable buildable)
            {
                if (queueJobGlobal)
                {
                    var collectJob = new Job
                    {
                        description = $"Collect resources for {selectedSchematic.entity.name}",
                    };

                    matchingInventories.ForEach
                    (
                        match =>
                        {
                            collectJob.orders.Add
                            (
                                new Order
                                {
                                    description = $"gather {match.itemAmount.amount} {match.itemAmount.item}(s) for {conveyorRecipe.name}",
                                    location = match.inventory.position,
                                    action = () =>
                                    {
                                        match.inventory.Withdraw(match.itemAmount);
                                        collectJob.activeWorker.Deposit(match.itemAmount);
                                        return Task.CompletedTask;
                                    },
                                }
                            );
                        }
                    );
                    
                    var deliveryJob = new Job
                    {
                        description = $"deliver items to {conveyor.name}",
                        prerequisite = collectJob,
                    };

                    deliveryJob.orders.Add
                    (
                        new Order
                        {
                            description = $"deliver items to {conveyor.name}",
                            location = conveyor.Center,
                            action = () =>
                            {
                                var deliveryItems = new List<Stack>();
                                conveyorRecipe.inputItems.ForEach
                                (
                                    itemAmount =>
                                    {
                                        var withdrawnItems = deliveryJob.activeWorker.Withdraw(itemAmount);
                                        deliveryItems.Add(withdrawnItems);
                                    }
                                );
                                return buildable.DeliverItems(deliveryItems);
                            },
                        }
                    );
                    JobManager.QueueJob(deliveryJob);
                    
                    JobManager.QueueJob(new Job
                    {
                        description = $"build a {conveyor.name}",
                        prerequisite = deliveryJob,
                        orders =
                        {
                            new Order
                            {
                                description = $"build a {conveyor.name}",
                                location = conveyor.Center,
                                action = () => buildable.Build(1),
                            },
                        },
                    });
                }
                else buildable.Build(0);
            }
        }

        private Node PlaceNode(Node selectedNode, Recipe nodeRecipe, Cell cell, bool queueJob = true)
        {
            Node node = (selectedNode) switch
            {
                Junction _ => Factory.Spawn("Junctions", selectedNode, cell.position),
                Machine _ => Factory.Spawn("Machines", selectedNode, cell.position), 
                _ => Factory.Spawn("Junctions", selectedNode, cell.position),
            };

            cell.node = node;
            node.parentCell = cell;

            if (node is Buildable buildable)
            {
                if (queueJob && queueJobGlobal)
                {
                    JobManager.QueueConstructionJob(cell, nodeRecipe, buildable, inventories);
                }
                else buildable.Build(0);
            }
            return node;
        }

        //> BUILD STUFF
        private void Update()
        {
            //- return is paused
            if (Time.timeScale == 0f) return;

            if (Input.GetKeyDown(KeyCode.Return))
            {
                queueJobGlobal = !queueJobGlobal;
            }
            
            //- toggle build mode
            if (Input.GetKeyDown(KeyCode.B))
            {
                buildMode = !buildMode;
                OnBuildModeChanged?.Invoke(buildMode);
            }
            
            //- ignore if over UI
            if (!buildMode || selectedSchematic is null || EventSystem.current.IsPointerOverGameObject()) return;
            
            //- handle conveyor building
            if (conveyorMode)
            {
                //- left click pressed
                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    firstNode = secondNode = null;
                    firstCell = CellGrid.GetCellUnderMouse();
                }

                //- left click released
                if (Input.GetKeyUp(KeyCode.Mouse0))
                {
                    if (firstCell is null || !firstCell.buildable)
                    {
                        Debug.Log("Invalid parentCell!");
                        return;
                    }
                    firstNode = (firstCell.occupied) ? firstCell.node : PlaceNode(defaultJunction, defaultJunctionRecipe, firstCell);
                    if (firstNode is null) return;
                    
                     
                    secondCell = CellGrid.GetCellUnderMouse();
                    if (firstCell is null || secondCell is null || !secondCell.buildable) Debug.Log("Invalid parentCell!");
                    else
                    {
                        secondNode = (secondCell.occupied) ? secondCell.node : PlaceNode(defaultJunction, defaultJunctionRecipe, secondCell);

                        //- if conveyor end points not parallel
                        if ((firstCell.coords.x == secondCell.coords.x) == (firstCell.coords.y == secondCell.coords.y))
                        {
                            var cell1 = CellGrid.GetCellAtCoordinates(new Vector2Int(firstCell.coords.x, secondCell.coords.y));
                            var cell2 = CellGrid.GetCellAtCoordinates(new Vector2Int(secondCell.coords.x, firstCell.coords.y));

                            Cell bestCell = cell1;
                            
                            if (!cell1.buildable || cell1.occupied)
                            {
                                Debug.Log("CELL 1 INVALID");
                                bestCell = cell2;
                            }

                            if (!cell2.buildable || cell2.occupied)
                            {
                                Debug.Log("CELL 2 INVALID");
                                return;
                            }

                            var junction = PlaceNode(defaultJunction, defaultJunctionRecipe, bestCell);
                            
                            BuildEdgeBetween(selectedSchematic.recipe, firstNode, junction);
                            BuildEdgeBetween(selectedSchematic.recipe, junction, secondNode);
                        }
                        else
                        {
                            BuildEdgeBetween(selectedSchematic.recipe, firstNode, secondNode);
                        }
                    }
                }
            }
            
            //- regular node building
            else if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                var cell = CellGrid.GetCellUnderMouse();
                if (cell is null)
                {
                    Debug.Log("Cell did not exist!");
                    return;
                }
                if (!cell.buildable)
                {
                    Debug.Log("Not valid cell to build on...");
                    return;
                }
                if (cell.occupied && cell.node is Junction )
                {
                    //@ issue here when replacing junctions
                    Debug.Log("Failed to replace junction with machine...");
                    return;

                    // IO input = default;
                    // IO output = default;
                    // if (cell.node.Input is { })
                    // {
                    //     input = cell.node.Input;
                    //     cell.node.DisconnectInput(cell.node.Input);
                    // }
                    // if (cell.node.Output is { })
                    // {
                    //     output = cell.node.Output;
                    //     cell.node.DisconnectOutput(cell.node.Output);
                    // }
                    // Destroy(cell.node);
                    //
                    // var newNode = PlaceNode(selectedPart.entity as Node, selectedPart.recipe, cell);
                    //
                    // if (input is {}) newNode.ConnectInput(input);
                    // if (output is {}) newNode.ConnectOutput(output);

                }

                if (!cell.occupied) PlaceNode(selectedSchematic.entity as Node, selectedSchematic.recipe, cell);
            }
            
            //- right click delete
            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                var cell = CellGrid.GetCellUnderMouse();
                if (cell is null)
                {
                    Debug.Log("Cell did not exist!");
                    return;
                }
                if (!cell.occupied)
                {
                    Debug.Log("Nothing present on selected cell.");
                    return;
                }

                if (cell.node is Buildable node)
                {
                    RemoveEntity(cell, node);
                }
                
                if (cell.edges.Count == 1 && cell.edges[0] is Buildable conveyor)
                {
                    RemoveEntity(cell, conveyor);
                }
                
            }
        }

        private void RemoveEntity(Cell cell, Buildable buildable)
        {
            if (queueJobGlobal)
            {
                JobManager.QueueJob(new Job
                {
                    orders = { new Order
                    {
                        description = $"destroy an entity",
                        location = cell.position,
                        action = () => buildable.Disassemble(1),
                    }},
                });
            }
            else buildable.Disassemble(0);
        }
    }
}
