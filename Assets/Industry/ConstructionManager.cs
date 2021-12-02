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
        [SerializeField] private List<Machine> builtMachines = new List<Machine>();
        [SerializeField] private List<IInventory> inventories = new List<IInventory>();

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
            // for (int i = 0; i < 64; i++)
            // {
            //     var spawnPosition = CellSpawner.GenerateRandomSpawn(c => c.buildable, startPosition.FloorToInt(), 100);
            //     ItemFactory.SpawnContainer(stone, spawnPosition);
            // }
            
            var ironIngot = ItemFactory.GetItem("Iron Ingot");
            // for (int i = 0; i < 32; i++)
            // {
            //     var spawnPosition = CellSpawner.GenerateRandomSpawn(c => c.buildable, startPosition.FloorToInt(), 100);
            //     ItemFactory.SpawnContainer(ironIngot, spawnPosition);
            // }
            
            var ironPlate = ItemFactory.GetItem("Iron Plate");
            // for (int i = 0; i < 32; i++)
            // {
            //     var spawnPosition = CellSpawner.GenerateRandomSpawn(c => c.buildable, startPosition.FloorToInt(), 100);
            //     ItemFactory.SpawnContainer(ironPlate, spawnPosition);
            // }

            var coalOre = ItemFactory.GetItem("Coal Ore");
            // for (int i = 0; i < 32; i++)
            // {
            //     var spawnPosition = CellSpawner.GenerateRandomSpawn(c => c.buildable, startPosition.FloorToInt(), 100);
            //     ItemFactory.SpawnContainer(coalOre, spawnPosition);
            // }

            var ironBeam = ItemFactory.GetItem("Iron Beam");
            // for (int i = 0; i < 32; i++)
            // {
            //     var spawnPosition = CellSpawner.GenerateRandomSpawn(c => c.buildable, startPosition.FloorToInt(), 100);
            //     ItemFactory.SpawnContainer(ironBeam, spawnPosition);
            // }

            var ironGear = ItemFactory.GetItem("Iron Gear");
            
            

            var storage = ItemFactory.GetItem("Storage Container");
            if (storage is Schematic part)
            {
                for (int i = 0; i < 1; i++)
                {
                    var cell = CellGrid.GetCellAtPosition(CellSpawner.GenerateRandomSpawn(c => c.buildable, startPosition.FloorToInt(), 25));
                    var node = BuildNodeOn(part.entity as Node, part.recipe, cell, false);
                    if (node is IBuildable buildable) buildable.Build(0);

                    if (node is IInventory inventory)
                    {
                        inventories.Add(inventory);
                        
                        inventory.Deposit(new ItemAmount
                        {
                            item = ironPlate,
                            amount = 128,
                        });
                        
                        inventory.Deposit(new ItemAmount
                        {
                            item = coalOre,
                            amount = 128,
                        });
                        
                        inventory.Deposit(new ItemAmount
                        {
                            item = ironBeam,
                            amount = 128,
                        });
                        
                        inventory.Deposit(new ItemAmount
                        {
                            item = ironIngot,
                            amount = 128,
                        });
                        
                        inventory.Deposit(new ItemAmount
                        {
                            item = stone,
                            amount = 128,
                        });
                        
                        inventory.Deposit(new ItemAmount
                        {
                            item = ironGear,
                            amount = 128,
                        });
                    }
                    
                }
            }
            
            var machines = FindObjectsOfType<Machine>();
            machines.ForEach(m => builtMachines.Add(m));
        }

        //> BUILD EDGE BETWEEN TWO NODES
        private void BuildEdgeBetween(Recipe conveyorRecipe, Node firstNode, Node secondNode)
        {
            if (!(selectedSchematic.entity is Edge selectedEdge)) return;
            
            bool cannotBuild = false;
            // var matchingEntities = new List<Entity>();
            // var requiredContainers = new List<Container>();
            var matchingInventories = new List<(IInventory inventory, ItemAmount itemAmount)>();
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
            
            firstNode.ConnectOutput(conveyor);
            conveyor.ConnectInput(firstNode);
            conveyor.ConnectOutput(secondNode);
            secondNode.ConnectInput(conveyor);

            var conveyorPath = AStar.FindPath(firstCell.position, secondCell.position, true);
            conveyorPath.ForEach(p =>
            {
                var tile = CellGrid.GetCellAtPosition(p);
                tile.edges.Add(conveyor);
                if (conveyor is Conveyor c) c.tilesCovered.Add(tile);
            });

            if (conveyor is IBuildable buildable)
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
                                var deliveryItems = new List<ItemAmount>();
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
                
                // JobManager.QueueJob(new Order
                // {
                //     description = $"build a {conveyor.name}",
                //     location = conveyor.Center,
                //     action = () => buildable.Build(1),
                // });
                // JobManager.QueueJob(new ConstructionJob(conveyor.Center, buildable, 1));
            }
        }

        private Node BuildNodeOn(Node selectedNode, Recipe nodeRecipe, Cell cell, bool queueJob = true)
        {
            // if (!(selectedPart.entity is Node selectedNode)) return default;

            bool cannotBuild = false;
            // var matchingEntities = new List<Entity>();
            // var requiredContainers = new List<Container>();
            var matchingInventories = new List<(IInventory inventory, ItemAmount itemAmount)>();
            if (queueJob && queueJobGlobal)
            {
                nodeRecipe.inputItems.ForEach(itemAmount =>
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
                                               .OrderBy(inv => Vector3.Distance(cell.position, inv.position))
                                               .FirstOrDefault();

                    if (bestMatch is null) cannotBuild = true;
                   
                    matchingInventories.Add((bestMatch, itemAmount));
                });
            }

            if (cannotBuild)
            {
                Debug.Log($"Unable to build {selectedSchematic.entity.name}");
                return default;
            }
            
            // requiredContainers.ForEach(c => matchingEntities.Add(c));
            // matchingInventories.ForEach((m => matchingEntities.Add(m)));
            // matchingEntities = matchingEntities.OrderBy(e => Vector3.Distance(cell.position, e.position)).ToList();

            // Node node = (selectedNode) switch
            // {
            //     Machine m  => Factory.Spawn("Machines", selectedNode, cell.position),
            //     Junction j => Factory.Spawn("Junctions", selectedNode, cell.position),
            //          _     => Factory.Spawn("Nodes", selectedNode, cell.position),
            // };

            Node node;
            if (selectedNode is Machine) node = Factory.Spawn("Machines", selectedNode, cell.position);
            else if (selectedNode is Junction) node = Factory.Spawn("Junctions", selectedNode, cell.position);
            else node = Factory.Spawn("Nodes", selectedNode, cell.position);
            
            cell.node = node;
            node.parentCell = cell;
            
            if (node is IBuildable buildable)
            {
                if (queueJob && queueJobGlobal)
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
                                    description = $"gather {match.itemAmount.amount} {match.itemAmount.item}(s) for {nodeRecipe.name}",
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

                    // requiredContainers.ForEach(
                    //     c =>
                    //     {
                    //         // if (e is Container c)
                    //         {
                    //             collectJob.orders.Add(new Order
                    //             {
                    //                 description = $"gather {c.item.name} for {nodeRecipe.name}",
                    //                 location = c.position,
                    //                 action = () =>
                    //                 {
                    //                     collectJob.activeWorker.Deposit(new ItemAmount{item = c.item, amount = 1});
                    //                     ItemFactory.DisposeContainer(c);
                    //                     return Task.CompletedTask;
                    //                 },
                    //             });
                    //         }
                    //         // if (e is Machine m)
                    //         // {
                    //         //     collectJob.orders.Add(new Order
                    //         //     {
                    //         //         description = $"gather {"temp"} for {nodeRecipe.name}",
                    //         //         location = m.position,
                    //         //         action = () =>
                    //         //         {
                    //         //             collectJob.activeWorker.Deposit(new ItemAmount{item = c.item, amount = 1});
                    //         //             ItemFactory.DisposeContainer(c);
                    //         //             return Task.CompletedTask;
                    //         //         },
                    //         //     });
                    //         // }
                    //     });

                    JobManager.QueueJob(collectJob);


                    var deliveryJob = new Job
                    {
                        description = $"deliver items to {node.name}",
                        prerequisite = collectJob,
                    };

                    deliveryJob.orders.Add
                    (
                        new Order
                        {
                            description = $"deliver items to {node.name}",
                            location = cell.position,
                            action = () =>
                            {
                                var deliveryItems = new List<ItemAmount>();
                                nodeRecipe.inputItems.ForEach
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

                    var constructionJob = new Job
                    {
                        description = $"build a {node.name}",
                        prerequisite = deliveryJob,
                        orders =
                        {
                            new Order
                            {
                                description = $"build a {node.name}",
                                // prerequisite = deliveryJob,
                                location = cell.position,
                                action = () => buildable.Build(1),
                            }
                        }
                    };
                    JobManager.QueueJob(constructionJob);
                    // JobManager.QueueJob(new ConstructionJob(cell.position, buildable, 1));
                }
                else buildable.Build(0);
            }
            
            if (node is Machine machine) builtMachines.Add(machine);

            return node;
        }

        private List<Machine> FindMachinesWithItems(List<ItemAmount> items)
        {
            List<Machine> matchingMachines = new List<Machine>();
            items.ForEach(i =>
            {
                var machine = builtMachines.FirstOrDefault(m => m.inputInventory.Contains(i) || m.outputInventory.Contains(i));
                if (machine is {}) matchingMachines.Add(machine);
            });
            return matchingMachines;
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
                    firstNode = (firstCell.occupied) ? firstCell.node : BuildNodeOn(defaultJunction, defaultJunctionRecipe, firstCell);
                    
                    if (firstNode is null) return;
                     
                    secondCell = CellGrid.GetCellUnderMouse();
                    if (firstCell is null || secondCell is null || !secondCell.buildable) Debug.Log("Invalid parentCell!");
                    else
                    {
                        secondNode = (secondCell.occupied) ? secondCell.node : BuildNodeOn(defaultJunction, defaultJunctionRecipe, secondCell);

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

                            var junction = BuildNodeOn(defaultJunction, defaultJunctionRecipe, bestCell);
                            // bestCell.node = junction;
                            // junction.parentCell = bestCell;
                            
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
                    Debug.Log("Replacing junction with machine...");
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
                    // var newNode = BuildNodeOn(selectedPart.entity as Node, selectedPart.recipe, cell);
                    //
                    // if (input is {}) newNode.ConnectInput(input);
                    // if (output is {}) newNode.ConnectOutput(output);

                }
                else if (!cell.occupied)
                {
                    var node = BuildNodeOn(selectedSchematic.entity as Node, selectedSchematic.recipe, cell);
                    if (node is null)
                    {
                        Debug.Log("Node was null.");
                        return;
                    }
                }
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

                if (cell.node is IBuildable node)
                {
                    if (queueJobGlobal)
                    {
                        JobManager.QueueJob(new Job
                        {
                            orders =
                            {
                                new Order
                                {
                                    description = $"destroy a {cell.node.name}",
                                    location = cell.position,
                                    action = () => node.Disassemble(1),
                                },
                            },
                        });
                    }
                    else node.Disassemble(0);
                }
                else if (cell.edges.Count == 1 && cell.edges[0] is IBuildable conveyor)
                {
                    if (queueJobGlobal)
                    {
                        JobManager.QueueJob(new Job
                        {
                            orders =
                            {
                                new Order
                                {
                                    description = $"destroy a {cell.edges[0].name}",
                                    location = cell.position,
                                    action = () => conveyor.Disassemble(1),
                                },
                            },
                        });
                    }
                    else conveyor.Disassemble(0);
                }
                
            }
        }
    }
}
