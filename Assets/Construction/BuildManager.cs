using System;
using UnityEngine;
using UnityEngine.EventSystems;
using Input = UnityEngine.Input;
using ProcessControl.Tools;
using ProcessControl.Graphs;
using ProcessControl.Industry.Conveyors;
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
        
        private void OnSetConveyorMode(bool truth) => conveyorMode = truth;
        private void OnSetNode(Node newSelection) => selectedNode = newSelection;
        private void OnSetEdge(Edge newSelection) => selectedEdge = newSelection;
        private Node BuildNodeAt(Cell cell) => Factory.Spawn("Nodes", selectedNode, cell.position);
        private Edge BuildEdge(Cell cell) => Factory.Spawn("Edges", selectedEdge, cell.position);
        
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
            
            //- handle conveyor building
            if (conveyorMode)
            {
                //- left click pressed
                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    firstNode = secondNode = null;
                    
                    var firstCell = ProceduralGrid.GetCellPosition(camera.MousePosition2D());
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
                    var secondCell = ProceduralGrid.GetCellUnderMouse();
                    if (secondCell is null || !secondCell.buildable) Debug.Log("Invalid parentCell!");
                    else
                    {
                        if (!secondCell.occupied)
                        {
                            secondNode = secondCell.node = BuildNodeAt(secondCell);
                            secondNode.parentCell = secondCell;
                        }
                        else secondNode = secondCell.node;
                        
                        // if (!firstNode.AvailableOutput) return;
                        // if (!secondNode.AvailableInput) return;
                        
                        var conveyor = Factory.Spawn("Edges", selectedEdge, Node.Center(firstNode, secondNode));

                        firstNode.ConnectOutput(conveyor);
                        conveyor.ConnectInput(firstNode);
                        conveyor.ConnectOutput(secondNode);
                        secondNode.ConnectInput(conveyor);
                    }
                }
            }
            
            //- regular node building
            else if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                var firstCell = ProceduralGrid.GetCellUnderMouse();

                if (firstCell is null || !firstCell.buildable) Debug.Log("Invalid parentCell!");
                else
                {
                    firstNode = (firstCell.occupied) ? firstCell.node: BuildNodeAt(firstCell);
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
                Debug.Log("Deleting Cell...");
                
                var cell = ProceduralGrid.GetCellUnderMouse();
                if (cell is null) Debug.Log("NO CELL FOUND!");
                else if (cell.occupied)
                {
                    Destroy(cell.node);
                    cell.node = null;
                }
            }
        }
    }
}
