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
        public Node BuildNode(Cell cell) => Factory.Spawn("Nodes", selectedNode, cell.center);
        // public Edge BuildEdge(ProceduralGrid.Cell cell) => Factory.Spawn("Edges", selectedEdge, c)
        
        private void Awake()
        {
            camera = Camera.main;

            SetNode += OnSetNode;
            SetEdge += OnSetEdge;
            SetEdgeMode += OnSetConveyorMode;
        }


        private void Update()
        {
            
            
            if (Input.GetKeyDown(KeyCode.B))
            {
                buildMode = !buildMode;
                OnBuildModeChanged?.Invoke(buildMode);
            }
            
            if (!buildMode || selectedNode is null || EventSystem.current.IsPointerOverGameObject()) return;
            

            if (conveyorMode)
            {
                
                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    firstNode = secondNode = null;
                    
                    var firstCell = ProceduralGrid.GetCellPosition(camera.MouseWorldPosition2D());
                    if (firstCell is null)
                    {
                        Debug.Log("NO CELL FOUND!");
                        return;
                    }
                    
                    if (!firstCell.occupied)
                    {
                        firstNode = firstCell.node = BuildNode(firstCell);
                        firstNode.Cell = firstCell;
                    }
                    else firstNode = firstCell.node;
                }

                if (Input.GetKeyUp(KeyCode.Mouse0))
                {
                    var secondCell = ProceduralGrid.GetCellPosition(camera.MouseWorldPosition2D());
                    if (secondCell is null) Debug.Log("NO CELL FOUND!");
                    else
                    {
                        if (!secondCell.occupied)
                        {
                            secondNode = secondCell.node = BuildNode(secondCell);
                            secondNode.Cell = secondCell;
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
            else if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                var firstCell = ProceduralGrid.GetCellUnderMouse();
                if (firstCell is null) Debug.Log("NO CELL FOUND!");
                else
                {
                    firstNode = (firstCell.occupied) ? firstCell.node: BuildNode(firstCell);
                    firstCell.node = firstNode;
                    firstNode.Cell = firstCell;

                    if (firstNode is Merger || firstNode is Splitter)
                    {
                        // var newMachine = BuildNode(firstCell);
                        // newMachine.ConnectOutput(firstNode.machine.currentOutput);
                        // firstNode.machine.input.ConnectInput(newMachine);
                        // Destroy(firstNode);
                    }
                }
            }

            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                var cell = ProceduralGrid.GetCellUnderMouse();
                if (cell is null) Debug.Log("NO CELL FOUND!");
                else
                if (cell.occupied)
                {
                    Destroy(cell.node);
                    cell.node = null;
                }
            }
        }


        private void OnDrawGizmos()
        {
            if (buildMode)
            {
                Gizmos.color = Color.white;
                var cell = ProceduralGrid.GetCellPosition(camera.MouseWorldPosition2D());
                if (cell is null) return;
                Gizmos.DrawWireCube(cell.center, Vector3.one);
            }
        }
    }
}
