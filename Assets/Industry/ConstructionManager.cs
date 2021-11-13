using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using Input = UnityEngine.Input;
using ProcessControl.Jobs;
using ProcessControl.Tools;
using ProcessControl.Graphs;
using ProcessControl.Procedural;
using ProcessControl.Pathfinding;
using Random = UnityEngine.Random;

#pragma warning disable 108,114


namespace ProcessControl.Industry
{
    public class ConstructionManager : MonoBehaviour
    {
        //> EVENT TRIGGERS
        public static Action<Part> SetPart;
        public static Action<bool> SetEdgeMode;

        //> EVENT SUBSCRIPTIONS
        public static Action<bool> OnBuildModeChanged;
        
        [SerializeField] private bool buildMode;
        [SerializeField] private bool conveyorMode;
        [SerializeField] private Part selectedPart;
        
        private Camera camera;
        private Vector2 startPosition;
        public Node firstNode, secondNode;
        public Cell firstCell, secondCell;
        [SerializeField] private List<Machine> builtMachines = new List<Machine>();



        //> INITIALIZATION
        private void Awake()
        {
            camera = Camera.main;

            SetPart += (part) => selectedPart = part;
            // SetNode += (newSelection) => selectedNode = newSelection;
            // SetEdge += (newSelection) => selectedEdge = newSelection;
            SetEdgeMode += (edgeMode) => conveyorMode = edgeMode;

            TileGrid.onStartLocationDetermined += GenerateSpawnArea;
        }

        private void GenerateSpawnArea(Vector2 position)
        {
            startPosition = position;


            var itemFactory = ItemFactory.Instance;
            var ironIngot = itemFactory.GetItem("Iron Ingot");
            var ironPlate = itemFactory.GetItem("Iron Plate");

            for (int i = 0; i < 25; i++)
            {
                var pos = Random.insideUnitCircle * (Random.value * 50);
                var cell = TileGrid.GetCellAtPosition(pos);
                if (!cell.buildable) continue;
                itemFactory.SpawnContainer(ironIngot, pos);
            }
            
            for (int i = 0; i < 25; i++)
            {
                var pos = Random.insideUnitCircle * (Random.value * 50);
                var cell = TileGrid.GetCellAtPosition(pos);
                if (!cell.buildable) continue;
                itemFactory.SpawnContainer(ironPlate, pos);
            }

            var storage = itemFactory.GetItem("Storage Container");
            if (storage is Part part)
            {
                for (int i = 0; i < 2; i++)
                {
                    var cell = TileGrid.GetCellAtPosition(Random.insideUnitCircle * (Random.value * 50));
                    var node = BuildNodeOn(part.entity as Node, part.recipe, cell, false);
                    if (node is IBuildable buildable) buildable.Build(0);
                }
            }
            
            var machines = FindObjectsOfType<Machine>();
            machines.ForEach(m => builtMachines.Add(m));
        }
        
        //> BUILD EDGE BETWEEN TWO NODES
        private void BuildEdgeBetween(Node firstNode, Node secondNode)
        {
            if (!(selectedPart.entity is Edge selectedEdge)) return;
            
            firstCell = firstNode.parentCell;
            secondCell = secondNode.parentCell;
                
            if ((firstCell.coords.x == secondCell.coords.x) == (firstCell.coords.y == secondCell.coords.y))
            {
                Debug.Log("Conveyors node not in straight line...");
                return;
            }

            var conveyor = Factory.Spawn("Conveyors", selectedEdge, Node.Center(firstNode, secondNode));
            firstNode.ConnectOutput(conveyor);
            conveyor.ConnectInput(firstNode);
            conveyor.ConnectOutput(secondNode);
            secondNode.ConnectInput(conveyor);

            var conveyorPath = AStar.FindPath(firstCell.position, secondCell.position);
            conveyorPath.ForEach(p =>
            {
                var tile = TileGrid.GetCellAtPosition(p);
                tile.edges.Add(conveyor);
                if (conveyor is Conveyor c) c.tilesCovered.Add(tile);
            });

            if (conveyor is IBuildable buildable)
            {
                AgentManager.QueueJob(new Job
                {
                    description = $"build a {conveyor.name}",
                    location = conveyor.Center,
                    order = () => buildable.Build(1),
                });
                // AgentManager.QueueJob(new ConstructionJob(conveyor.Center, buildable, 1));
            }
        }

        private Node BuildNodeOn(Node selectedNode, Recipe nodeRecipe, Cell cell, bool queueJob = true)
        {
            // if (!(selectedPart.entity is Node selectedNode)) return default;
            
            Node node;
            if (selectedNode is Machine) node = Factory.Spawn("Machines", selectedNode, cell.position);
            else if (selectedNode is Junction) node = Factory.Spawn("Junctions", selectedNode, cell.position);
            else node = Factory.Spawn("Nodes", selectedNode, cell.position);
            
            cell.node = node;
            node.parentCell = cell;
            
            if (node is IBuildable buildable)
            {
                if (queueJob)
                {
                    // var collectJob = new Job
                    // {
                    //     description = $"gather items for {nodeRecipe.name}",
                    //     order = () =>
                    //     {
                    //         
                    
                    //     },
                    // };
                    var deliveryJob = new Job
                    {
                        description = $"deliver items to {node.name}",
                        location = cell.position,
                        order = () =>
                        {
                            buildable.DeliverItems(nodeRecipe.inputItems);
                            return Task.CompletedTask;
                        },
                    };
                    AgentManager.QueueJob(deliveryJob);
                    
                    AgentManager.QueueJob(new Job
                    {
                        description = $"build a {node.name}",
                        prerequisite = deliveryJob,
                        location = cell.position,
                        order = () => buildable.Build(1),
                    });
                    // AgentManager.QueueJob(new ConstructionJob(cell.position, buildable, 1));
                }
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
            
            //- toggle build mode
            if (Input.GetKeyDown(KeyCode.B))
            {
                buildMode = !buildMode;
                OnBuildModeChanged?.Invoke(buildMode);
            }
            
            //- ignore if over UI
            if (!buildMode || selectedPart is null || EventSystem.current.IsPointerOverGameObject()) return;
            
            //- handle conveyor building
            if (conveyorMode)
            {
                //- left click pressed
                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    firstNode = secondNode = null;
                    firstCell = TileGrid.GetCellUnderMouse();
                }

                //- left click released
                if (Input.GetKeyUp(KeyCode.Mouse0))
                {
                    if (firstCell is null || !firstCell.buildable)
                    {
                        Debug.Log("Invalid parentCell!");
                        return;
                    }
                    firstNode = (firstCell.occupied) ? firstCell.node : BuildNodeOn(selectedPart.entity as Node, selectedPart.recipe, firstCell);
                    
                    if (firstNode is null) return;
                     
                    secondCell = TileGrid.GetCellUnderMouse();
                    if (firstCell is null || secondCell is null || !secondCell.buildable) Debug.Log("Invalid parentCell!");
                    else
                    {
                        secondNode = (secondCell.occupied) ? secondCell.node : BuildNodeOn(selectedPart.entity as Node, selectedPart.recipe, secondCell);

                        //- if conveyor end points not parallel
                        if ((firstCell.coords.x == secondCell.coords.x) == (firstCell.coords.y == secondCell.coords.y))
                        {
                            var cell1 = TileGrid.GetCellAtCoordinates(new Vector2Int(firstCell.coords.x, secondCell.coords.y));
                            var cell2 = TileGrid.GetCellAtCoordinates(new Vector2Int(secondCell.coords.x, firstCell.coords.y));

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

                            var junction = BuildNodeOn(selectedPart.entity as Node, selectedPart.recipe, bestCell);
                            // bestCell.node = junction;
                            // junction.parentCell = bestCell;
                            
                            BuildEdgeBetween(firstNode, junction);
                            BuildEdgeBetween(junction, secondNode);
                        }
                        else
                        {
                            BuildEdgeBetween(firstNode, secondNode);
                        }
                    }
                }
            }
            
            //- regular node building
            else if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                var cell = TileGrid.GetCellUnderMouse();
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

                    IO input = default;
                    IO output = default;
                    if (cell.node.Input is { })
                    {
                        input = cell.node.Input;
                        cell.node.DisconnectInput(cell.node.Input);
                    }
                    if (cell.node.Output is { })
                    {
                        output = cell.node.Output;
                        cell.node.DisconnectOutput(cell.node.Output);
                    }
                    Destroy(cell.node);
                    
                    var newNode = BuildNodeOn(selectedPart.entity as Node, selectedPart.recipe, cell);

                    if (input is {}) newNode.ConnectInput(input);
                    if (output is {}) newNode.ConnectOutput(output);

                }
                else if (!cell.occupied)
                {
                    var node = BuildNodeOn(selectedPart.entity as Node, selectedPart.recipe, cell);
                    if (node is null)
                    {
                        Debug.Log("Node was null.");
                        return;
                    }
                    // node.parentCell = cell;
                    // cell.node = node;
                }
            }
            
            //- right click delete
            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                var cell = TileGrid.GetCellUnderMouse();
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
                    AgentManager.QueueJob(new Job
                    {
                        description = $"destroy a {cell.node.name}",
                        location = cell.position,
                        order = () => node.Deconstruct(1),
                    });
                }
                else if (cell.edges.Count == 1 && cell.edges[0] is IBuildable conveyor)
                {
                    AgentManager.QueueJob(new Job
                    {
                        description = $"destroy a {cell.edges[0].name}",
                        location = cell.position,
                        order = () => conveyor.Deconstruct(1),
                    });
                }
                
            }
        }
    }
}