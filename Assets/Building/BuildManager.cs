using System;
using ProcessControl.Machines;
using UnityEngine;
using UnityEngine.EventSystems;
using ProcessControl.Tools;
using Grid = ProcessControl.Terrain.Grid;


namespace ProcessControl.Building
{
    public class BuildManager : MonoBehaviour
    {
        public Entity selectedEntity;

        [SerializeField] private bool conveyorMode;
        [SerializeField] private bool buildMode;
        new private Camera camera;
        
        //> EVENT TRIGGERS
        public static Action<Entity> SetBuildItem;
        public static Action<bool> SetConveyorMode;

        //> EVENT SUBSCRIPTIONS
        public static Action<bool> OnBuildModeChanged;
        
        private Entity firstEntity, secondEntity;
        private Machine previousMachine;
        
        private void OnSetConveyorMode(bool truth) => conveyorMode = truth;
        private void OnSetBuildItem(Entity newSelection) => selectedEntity = newSelection;
        public Entity BuildEntity(Grid.Cell cell) => Factory.Spawn("Entities", selectedEntity, cell.center);

        private void Awake()
        {
            camera = Camera.main;

            SetBuildItem += OnSetBuildItem;
            SetConveyorMode += OnSetConveyorMode;
        }


        private void Update()
        {
            
            
            if (Input.GetKeyDown(KeyCode.B))
            {
                buildMode = !buildMode;
                OnBuildModeChanged?.Invoke(buildMode);
            }
            
            if (!buildMode || !selectedEntity || EventSystem.current.IsPointerOverGameObject()) return;
            

            if (conveyorMode)
            {
                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    var firstCell = Grid.GetCellPosition(camera.MouseWorldPosition2D());
                    if (firstCell is null) Debug.Log("NO CELL FOUND!");
                    else
                    {
                        firstEntity = (firstCell.occupied) ? firstCell.entity : BuildEntity(firstCell);
                        firstCell.entity = firstEntity;
                        firstEntity.cell = firstCell;
                    }
                }

                if (Input.GetKeyUp(KeyCode.Mouse0))
                {
                    var secondCell = Grid.GetCellPosition(camera.MouseWorldPosition2D());
                    if (secondCell is null) Debug.Log("NO CELL FOUND!");
                    else
                    {
                        secondEntity = (secondCell.occupied) ? secondCell.entity : BuildEntity(secondCell);
                        secondCell.entity = secondEntity;
                        secondEntity.cell = secondCell;

                        var firstMachine = firstEntity.GetComponent<Machine>();
                        var secondMachine = secondEntity.GetComponent<Machine>();
                        
                        firstMachine.ConnectOutput(secondMachine.machine.input);
                        secondMachine.ConnectInput(firstMachine.machine.output);
                    }
                }
            }
            else if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                var firstCell = Grid.GetCellPosition(camera.MouseWorldPosition2D());
                if (firstCell is null) Debug.Log("NO CELL FOUND!");
                else
                {
                    firstEntity = (firstCell.occupied) ? firstCell.entity.gameObject.GetComponent<Machine>() : BuildEntity(firstCell);
                    firstCell.entity = firstEntity;
                    firstEntity.cell = firstCell;
                }
            }

            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                var cell = Grid.GetCellUnderMouse();
                if (cell is null) Debug.Log("NO CELL FOUND!");
                else
                if (cell.occupied)
                {
                    cell.entity.Delete();
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
