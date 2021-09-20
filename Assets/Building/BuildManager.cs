using System;
using UnityEngine;
using UnityEngine.EventSystems;
using ProcessControl.Tools;
using ProcessControl.Graphs;
using ProcessControl.Machines;
using Grid = ProcessControl.Terrain.Grid;
using Input = UnityEngine.Input;


namespace ProcessControl.Building
{
    public class BuildManager : MonoBehaviour
    {
        public Machine selectedMachine;
        public Conveyor selectedConveyor;

        [SerializeField] private bool buildMode;
        [SerializeField] private bool conveyorMode;
        
        new private Camera camera;
        
        //> EVENT TRIGGERS
        public static Action<Machine> SetMachine;
        public static Action<Conveyor> SetConveyor;
        public static Action<bool> SetConveyorMode;

        //> EVENT SUBSCRIPTIONS
        public static Action<bool> OnBuildModeChanged;
        
        public Machine firstMachine, secondMachine;
        private Machine previousMachine;
        
        private void OnSetConveyorMode(bool truth) => conveyorMode = truth;
        private void OnSetNode(Machine newSelection) => selectedMachine = newSelection;
        private void OnSetEdge(Conveyor newSelection) => selectedConveyor = newSelection;
        public Machine BuildMachine(Grid.Cell cell) => Factory.Spawn("Machines", selectedMachine, cell.center);

        private void Awake()
        {
            camera = Camera.main;

            SetMachine += OnSetNode;
            SetConveyor += OnSetEdge;
            SetConveyorMode += OnSetConveyorMode;
        }


        private void Update()
        {
            
            
            if (Input.GetKeyDown(KeyCode.B))
            {
                buildMode = !buildMode;
                OnBuildModeChanged?.Invoke(buildMode);
            }
            
            if (!buildMode || selectedMachine is null || EventSystem.current.IsPointerOverGameObject()) return;
            

            if (conveyorMode)
            {
                
                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    firstMachine = secondMachine = null;
                    
                    var firstCell = Grid.GetCellPosition(camera.MouseWorldPosition2D());
                    if (firstCell is null)
                    {
                        Debug.Log("NO CELL FOUND!");
                        return;
                    }
                    
                    if (!firstCell.occupied)
                    {
                        firstMachine = firstCell.machine = BuildMachine(firstCell);
                        firstMachine.node.cell = firstCell;
                    }
                    else firstMachine = firstCell.machine;
                }

                if (Input.GetKeyUp(KeyCode.Mouse0))
                {
                    var secondCell = Grid.GetCellPosition(camera.MouseWorldPosition2D());
                    if (secondCell is null) Debug.Log("NO CELL FOUND!");
                    else
                    {
                        if (!secondCell.occupied)
                        {
                            secondMachine = secondCell.machine = BuildMachine(secondCell);
                            secondMachine.node.cell = secondCell;
                        }
                        else secondMachine = secondCell.machine;
                        
                        if (!firstMachine.AvailableOutput) return;
                        if (!secondMachine.AvailableInput) return;
                        
                        var conveyor = Factory.Spawn("Edges", selectedConveyor, Node.Center(firstMachine, secondMachine));

                        firstMachine.ConnectOutput(conveyor);
                        conveyor.ConnectInput(firstMachine);
                        conveyor.ConnectOutput(secondMachine);
                        secondMachine.ConnectInput(conveyor);
                        
                        conveyor.SetLength(conveyor.conveyor.distanceBetweenIO);
                        var direction = conveyor.conveyor.input.DirectionTo(conveyor.conveyor.output);
                        var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                        conveyor.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
                    }
                }
            }
            else if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                var firstCell = Grid.GetCellUnderMouse();
                if (firstCell is null) Debug.Log("NO CELL FOUND!");
                else
                {
                    firstMachine = (firstCell.occupied) ? firstCell.machine: BuildMachine(firstCell);
                    firstCell.machine = firstMachine;
                    firstMachine.node.cell = firstCell;

                    if (firstMachine is Merger || firstMachine is Splitter)
                    {
                        var newMachine = BuildMachine(firstCell);
                        newMachine.ConnectOutput(firstMachine.Output);
                        firstMachine.Output.ConnectInput(newMachine);
                        Destroy(firstMachine);
                        
                    }
                }
            }

            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                var cell = Grid.GetCellUnderMouse();
                if (cell is null) Debug.Log("NO CELL FOUND!");
                else
                if (cell.occupied)
                {
                    Destroy(cell.machine);
                    cell.machine = null;
                }
            }
        }


        private void OnDrawGizmos()
        {
            if (buildMode)
            {
                Gizmos.color = Color.white;
                var cell = Grid.GetCellPosition(camera.MouseWorldPosition2D());
                if (cell is null) return;
                Gizmos.DrawWireCube(cell.center, Vector3.one);
            }
        }
    }
}
