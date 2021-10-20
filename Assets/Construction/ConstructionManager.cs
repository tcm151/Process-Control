using System;
using UnityEngine;
using UnityEngine.EventSystems;
using Input = UnityEngine.Input;
using ProcessControl.Jobs;
using ProcessControl.Tools;
using ProcessControl.Graphs;
using ProcessControl.Procedural;
using ProcessControl.Industry.Machines;
using ProcessControl.Industry.Conveyors;

#pragma warning disable 108,114


namespace ProcessControl.Construction
{
    public class ConstructionManager : MonoBehaviour
    {
        public Node selectedNode;
        public Edge selectedEdge;

        [SerializeField] private bool buildMode;
        [SerializeField] private bool conveyorMode;
        
        private Camera camera;
        
        //> EVENT TRIGGERS
        public static Action<Node> SetNode;
        public static Action<Edge> SetEdge;
        public static Action<bool> SetEdgeMode;

        //> EVENT SUBSCRIPTIONS
        public static Action<bool> OnBuildModeChanged;
        
        
        public Node firstNode, secondNode;
        public Cell firstCell, secondCell;
        
        private Edge BuildEdgeBetween(Node firstNode, Node secondNode) => Factory.Spawn("Edges", selectedEdge, Node.Center(firstNode, secondNode));
        private Node BuildNodeOn(Cell cell)
        {
            if (selectedNode is Machine) return Factory.Spawn("Machines", selectedNode, cell.position);
            if (secondNode is Junction) return Factory.Spawn("Junctions", selectedNode, cell.position);
            return Factory.Spawn("Nodes", selectedNode, cell.position);
        }
        
        //> INITIALIZATION
        private void Awake()
        {
            camera = Camera.main;

            SetNode += (newSelection) => selectedNode = newSelection;
            SetEdge += (newSelection) => selectedEdge = newSelection;
            SetEdgeMode += (edgeMode) => conveyorMode = edgeMode;
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
            if (!buildMode || selectedNode is null || EventSystem.current.IsPointerOverGameObject()) return;
            
            //- handle conveyor building
            if (conveyorMode)
            {
                //- left click pressed
                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    firstNode = secondNode = null;

                    firstCell = TileGrid.GetCellUnderMouse();
                    if (firstCell is null || !firstCell.buildable)
                    {
                        Debug.Log("Invalid parentCell!");
                        return;
                    }
                    
                    if (!firstCell.occupied)
                    {
                        firstNode = firstCell.node = BuildNodeOn(firstCell);
                        firstNode.parentCell = firstCell;
                    }
                    else firstNode = firstCell.node;
                }

                //- left click released
                if (Input.GetKeyUp(KeyCode.Mouse0))
                {
                    secondCell = TileGrid.GetCellUnderMouse();
                    if (firstCell is null || secondCell is null || !secondCell.buildable) Debug.Log("Invalid parentCell!");
                    else
                    {
                        
                        if (!secondCell.occupied)
                        {
                            secondNode = secondCell.node = BuildNodeOn(secondCell);
                            secondNode.parentCell = secondCell;
                        }
                        else secondNode = secondCell.node;
                        
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

                            var junction = BuildNodeOn(bestCell);
                            bestCell.node = junction;
                            junction.parentCell = bestCell;
                            
                            var conveyor1 = BuildEdgeBetween(firstNode, junction);
                            var conveyor2 = BuildEdgeBetween(junction, secondNode);
                            
                            firstNode.ConnectOutput(conveyor1);
                            conveyor1.ConnectInput(firstNode);
                            conveyor1.ConnectOutput(junction);
                            junction.ConnectInput(conveyor1);
                            junction.ConnectOutput(conveyor2);
                            conveyor2.ConnectInput(junction);
                            conveyor2.ConnectOutput(secondNode);
                            secondNode.ConnectInput(conveyor2);
                        }
                        else
                        {
                            var conveyor = BuildEdgeBetween(firstNode, secondNode);

                            firstNode.ConnectOutput(conveyor);
                            conveyor.ConnectInput(firstNode);
                            conveyor.ConnectOutput(secondNode);
                            secondNode.ConnectInput(conveyor);

                            // Job job1 = (firstNode) switch
                            // {
                            //     Machine m when !m.machine.enabled => new Job {
                            //         destination = firstCell,
                            //         action = m.Build,
                            //     },
                            //     
                            //     MultiJunction j when !j.junction.enabled => new Job {
                            //         destination = firstCell,
                            //         action = j.Build,
                            //     },
                            //     
                            //     _ => null,
                            // };
                            //
                            // Job job2 = (secondNode) switch
                            // {
                            //     Machine m when !m.machine.enabled => new Job
                            //     {
                            //         destination = firstCell,
                            //         action = m.Build,
                            //     },
                            //
                            //     MultiJunction j when !j.junction.enabled => new Job
                            //     {
                            //         destination = firstCell,
                            //         action = j.Build,
                            //     },
                            //
                            //     _ => null,
                            // };
                            //
                            // if (job1 is { }) AgentManager.QueueJob(job1);
                            // if (job2 is { }) AgentManager.QueueJob(job2);
                            //
                            // if (conveyor is Conveyor c)
                            // {
                            //     AgentManager.QueueJob(new Job
                            //     {
                            //         destination = TileGrid.GetCellAtPosition(c.transform.position),
                            //         action = c.Build,
                            //     });
                            // }
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
                if (cell.occupied)
                {
                    Debug.Log("Cell is already occupied.");
                    return;
                }
                
                
                var node = BuildNodeOn(cell);
                node.parentCell = cell;
                cell.node = node;

                // if (node is Junction )
                // {
                    //@ replace junctions with machines when applicable
                // }

                if (node is Machine machine)
                {
                    AgentManager.QueueJob(new Job
                    {
                        location = cell.position,
                        action = machine.Build,
                    });
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
                
                AgentManager.QueueJob(new Job
                {
                    location = cell.position,
                    action = () => Destroy(cell.node),
                });
            }
        }
    }
}
