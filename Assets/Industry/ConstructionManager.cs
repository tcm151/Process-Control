using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using ProcessControl.Jobs;
using ProcessControl.Tools;
using ProcessControl.Graphs;
using ProcessControl.Procedural;
using ProcessControl.Pathfinding;

#pragma warning disable 108,114


namespace ProcessControl.Industry
{
    public class ConstructionManager : Service
    {
        //> EVENT TRIGGERS
        public static Action<Schematic> SetPart;
        public static Action<bool> SetEdgeMode;

        //> EVENT SUBSCRIPTIONS
        public static Action<bool> OnBuildModeChanged;

        public bool godModEnabled = true;
        [SerializeField] private bool buildMode;
        [SerializeField] private bool conveyorMode;
        [SerializeField] private Recipe selectedRecipe;
        [SerializeField] private Schematic selectedSchematic;
        
        private Camera camera;
        public Node firstNode, secondNode;
        public Cell firstCell, secondCell;
        // [SerializeField] private List<Machine> builtMachines = new List<Machine>();
        [SerializeField] private List<Inventory> inventories = new List<Inventory>();

        public Schematic defaultJunction;
        public Recipe defaultJunctionRecipe;

        //> INITIALIZATION
        protected override void Awake()
        {
            base.Awake();
            
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
            if (storage is Schematic schematic)
            {
                for (int i = 0; i < 1; i++)
                {
                    var cell = CellGrid.GetCellAtPosition(CellSpawner.GenerateRandomSpawn(c => c.buildable, startPosition.FloorToInt(), 25));
                    var node = PlaceNode(schematic, cell, false);
                    if (node is Buildable buildable) buildable.Build(0);

                    if (node is IO io)
                    {
                        var inventory = io.InputInventory;
                        inventories.Add(inventory);
                        
                        inventory.Deposit(new Stack
                        {
                            item = ItemFactory.GetItem("Machine Frame"),
                            amount = 128,
                        });
                        
                        inventory.Deposit(new Stack
                        {
                            item = ItemFactory.GetItem("Stone Furnace"),
                            amount = 128,
                        });
                        
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
        }

        //> BUILD EDGE BETWEEN TWO NODES
        public void PlaceEdge(Recipe recipe, Node firstNode, Node secondNode, bool queueJob = true)
        {
            if (!(selectedSchematic.entity is Edge selectedEdge)) return;
            
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
                Debug.Log("NODES WERE NOT IO DEVICES...");
            }
            

            var conveyorPath = AStar.FindPath(firstCell.position, secondCell.position, true);
            conveyorPath.ForEach(p =>
            {
                var cell = CellGrid.GetCellAtPosition(p);
                cell.edges.Add(conveyor);
                if (conveyor is Conveyor c) c.tilesCovered.Add(cell);
            });
            
            if (conveyor is Buildable buildable)
            {
                if (queueJob && !godModEnabled)
                {
                    JobManager.QueueEdgeConstruction(firstCell, secondCell, recipe, buildable, inventories);
                }
                else buildable.Build(0);
            }
        }

        public Node PlaceNode(Schematic schematic , Cell cell, bool queueJob = true)
        {
            Node node = (schematic.entity) switch
            {
                Junction junction => Factory.Spawn("Junctions", junction, cell.position),
                Machine machine  => Factory.Spawn("Machines", machine, cell.position), 
                _               => Factory.Spawn("Nodes", schematic.entity as Node, cell.position),
            };

            cell.node = node;
            node.parentCell = cell;

            if (node is Buildable buildable)
            {
                if (queueJob && !godModEnabled)
                {
                    JobManager.QueueNodeConstruction(cell, schematic.recipe, buildable, inventories);
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
                godModEnabled = !godModEnabled;
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
                    firstNode = (firstCell.occupied) ? firstCell.node : PlaceNode(defaultJunction, firstCell);
                    if (firstNode is null) return;
                    
                     
                    secondCell = CellGrid.GetCellUnderMouse();
                    if (firstCell is null || secondCell is null || !secondCell.buildable) Debug.Log("Invalid parentCell!");
                    else
                    {
                        secondNode = (secondCell.occupied) ? secondCell.node : PlaceNode(defaultJunction, secondCell);

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

                            var junction = PlaceNode(defaultJunction, bestCell);
                            
                            PlaceEdge(selectedSchematic.recipe, firstNode, junction);
                            PlaceEdge(selectedSchematic.recipe, junction, secondNode);
                        }
                        else
                        {
                            PlaceEdge(selectedSchematic.recipe, firstNode, secondNode);
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

                if (!cell.occupied) PlaceNode(selectedSchematic, cell);
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

        private void RemoveEntity(Cell cell, Buildable buildable, bool queueJob = true)
        {
            if (queueJob)
            {
                JobManager.QueueJob(new Job
                {
                    orders = { new Order
                    {
                        description = $"destroy an entity",
                        location = cell.position,
                        action = () =>
                        {
                            buildable.recipe.inputItems.ForEach(stack =>
                            {
                                for (int i = 0; i < stack.amount; i++)
                                {
                                    ItemFactory.SpawnContainer(stack.item, cell.position);
                                }
                            });
                            return buildable.Disassemble(1);
                        },
                    }},
                });
            }
            else
            {
                // buildable.recipe.inputItems.ForEach(stack =>
                // {
                //     for (int i = 0; i < stack.amount; i++)
                //     {
                //         ItemFactory.SpawnContainer(stack.stack, cell.position);
                //     }
                // });
                buildable.Disassemble(0);
            }
        }
    }
}
