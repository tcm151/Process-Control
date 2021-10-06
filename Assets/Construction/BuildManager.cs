using System;
using UnityEngine;
using UnityEngine.EventSystems;
using Input = UnityEngine.Input;
using ProcessControl.Tools;
using ProcessControl.Graphs;
using ProcessControl.Industry.Conveyors;
using ProcessControl.Industry.Machines;
using ProcessControl.Procedural;


namespace ProcessControl.Construction
{
    public class BuildManager : MonoBehaviour
    {
        public Node selectedNode;
        public Edge selectedEdge;

        [SerializeField] private bool buildMode;
        [SerializeField] private bool conveyorMode;
        
        new private Camera camera;
        
        //> EVENT TRIGGERS
        public static Action<Node> SetNode;
        public static Action<Edge> SetEdge;
        public static Action<bool> SetEdgeMode;

        //> EVENT SUBSCRIPTIONS
        public static Action<bool> OnBuildModeChanged;
        
        public Node firstNode, secondNode;
        public Cell firstCell, secondCell;
        
        private void OnSetConveyorMode(bool truth) => conveyorMode = truth;
        private void OnSetNode(Node newSelection) => selectedNode = newSelection;
        private void OnSetEdge(Edge newSelection) => selectedEdge = newSelection;
        
        private Edge BuildEdge(Node firstNode, Node secondNode) => Factory.Spawn("Edges", selectedEdge, Node.Center(firstNode, secondNode));
        private Node BuildNodeAt(Cell cell)
        {
            if (selectedNode is Machine) return Factory.Spawn("Machines", selectedNode, cell.position);
            if (secondNode is Junction) return Factory.Spawn("Junctions", selectedNode, cell.position);
            return Factory.Spawn("Nodes", selectedNode, cell.position);
        }
        
        //> INITIALIZATION
        private void Awake()
        {
            camera = Camera.main;

            SetNode += OnSetNode;
            SetEdge += OnSetEdge;
            SetEdgeMode += OnSetConveyorMode;
        }

        //> BUILD STUFF
        private void Update()
        {
            //- toggle build mode
            if (Input.GetKeyDown(KeyCode.B))
            {
                buildMode = !buildMode;
                OnBuildModeChanged?.Invoke(buildMode);
            }
            
            if (!buildMode || selectedNode is null || EventSystem.current.IsPointerOverGameObject()) return;

            // firstNode = secondNode = null;
            // firstCell = secondCell = null;
            
            //- handle conveyor building
            if (conveyorMode)
            {
                //- left click pressed
                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    firstNode = secondNode = null;

                    firstCell = ProceduralGrid.GetCellUnderMouse();
                    if (firstCell is null || !firstCell.buildable)
                    {
                        Debug.Log("Invalid parentCell!");
                        return;
                    }
                    
                    if (!firstCell.occupied)
                    {
                        firstNode = firstCell.node = BuildNodeAt(firstCell);
                        firstNode.parentCell = firstCell;
                    }
                    else firstNode = firstCell.node;
                }

                //- left click released
                if (Input.GetKeyUp(KeyCode.Mouse0))
                {
                    secondCell = ProceduralGrid.GetCellUnderMouse();
                    if (firstCell is null || secondCell is null || !secondCell.buildable) Debug.Log("Invalid parentCell!");
                    else
                    {
                        
                        if (!secondCell.occupied)
                        {
                            secondNode = secondCell.node = BuildNodeAt(secondCell);
                            secondNode.parentCell = secondCell;
                        }
                        else secondNode = secondCell.node;
                        
                        if ((firstCell.coords.x == secondCell.coords.x) == (firstCell.coords.y == secondCell.coords.y))
                        {
                            Debug.Log("NOT STRAIGHT LINE CONVEYOR");
                            var cell1 = ProceduralGrid.GetCellCoords(new Vector2Int(firstCell.coords.x, secondCell.coords.y));
                            var cell2 = ProceduralGrid.GetCellCoords(new Vector2Int(secondCell.coords.x, firstCell.coords.y));

                            if (!cell1.buildable || cell1.occupied)
                            {
                                Debug.Log("CELL 1 INVALID");
                            }

                            if (!cell2.buildable || cell2.occupied)
                            {
                                Debug.Log("CELL 2 INVALID");
                            }

                            var junction = BuildNodeAt(cell1);
                            cell1.node = junction;
                            junction.parentCell = cell1;
                            
                            var conveyor1 = BuildEdge(firstNode, junction);
                            var conveyor2 = BuildEdge(junction, secondNode);
                            
                            firstNode.ConnectOutput(conveyor1);
                            conveyor1.ConnectInput(firstNode);
                            conveyor1.ConnectOutput(junction);
                            junction.ConnectInput(conveyor1);
                            conveyor2.ConnectInput(junction);
                            conveyor2.ConnectOutput(secondNode);
                            secondNode.ConnectInput(conveyor2);
                        }
                        else
                        {
                            var conveyor = BuildEdge(firstNode, secondNode);

                            firstNode.ConnectOutput(conveyor);
                            conveyor.ConnectInput(firstNode);
                            conveyor.ConnectOutput(secondNode);
                            secondNode.ConnectInput(conveyor);
                        }
                    }
                }
            }
            
            //- regular node building
            else if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                firstCell = ProceduralGrid.GetCellUnderMouse();

                if (firstCell is null || !firstCell.buildable) Debug.Log("Invalid parentCell!");
                else
                {
                    firstNode = (firstCell.occupied) ? firstCell.node : BuildNodeAt(firstCell);
                    firstNode.parentCell = firstCell;
                    firstCell.node = firstNode;

                    if (firstNode is Junction)
                    {
                        //@ replace junctions with machines when applicable
                    }
                }
            }
            
            
            
            //- right click delete
            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                var cell = ProceduralGrid.GetCellUnderMouse();
                if (cell is null) Debug.Log("NO CELL FOUND!");
                else if (cell.occupied)
                {
                    Debug.Log("Deleting Cell...");
                    Destroy(cell.node);
                    cell.node = null;
                }
            }
        }
    }
}
