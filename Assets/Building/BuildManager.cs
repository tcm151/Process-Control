using System;
using ProcessControl.Conveyors;
using ProcessControl.Tools;
using ProcessControl.Terrain;
using UnityEngine;
using UnityEngine.EventSystems;


namespace ProcessControl.Building
{
    public class BuildManager : MonoBehaviour
    {
        public Node currentBuildItem;

        private bool buildMode;
        new private Camera camera;
        
        public static Action<bool> OnBuildModeChanged;
        public static Action<Node> SetBuildItem;

        private Node firstNode, secondNode;

        private void Awake()
        {
            camera = Camera.main;

            SetBuildItem += OnSetBuildItem;
        }

        private void Update()
        {
            
            
            if (Input.GetKeyDown(KeyCode.B))
            {
                buildMode = !buildMode;
                OnBuildModeChanged?.Invoke(buildMode);
            }
            
            if (!buildMode || !currentBuildItem || EventSystem.current.IsPointerOverGameObject()) return;

            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                var firstCell = ProceduralGrid.GetCellPosition(camera.MouseWorldPosition2D());
                if (firstCell is null) Debug.Log("NO CELL FOUND!");
                else
                {
                    firstNode = (firstCell.occupied) ? firstCell.node : Factory.Spawn(currentBuildItem, firstCell.center);
                    firstCell.node = firstNode;
                }
            }

            if (Input.GetKey(KeyCode.Mouse0))
            {
                var cell = ProceduralGrid.GetCellPosition(camera.MouseWorldPosition2D());
                if (cell is null)  Debug.Log("NO CELL FOUND!");
                else
                {
                    var node = (cell.occupied) ? cell.node : Factory.Spawn(currentBuildItem, cell.center);
                }
            }
            
            if (Input.GetKeyUp(KeyCode.Mouse0))
            {
                var secondCell = ProceduralGrid.GetCellPosition(camera.MouseWorldPosition2D());
                if (secondCell is null) Debug.Log("NO CELL FOUND!");
                else
                {
                    secondNode = (secondCell.occupied) ? secondCell.node : Factory.Spawn(currentBuildItem, secondCell.center);
                    secondCell.node = secondNode;
                    
                    firstNode.AddConnection(secondNode);
                    secondNode.AddConnection(firstNode);
                }
            }

            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                var cell = ProceduralGrid.GetCellPosition(camera.MouseWorldPosition2D());
                if (cell is null) Debug.Log("NO CELL FOUND!");
                else
                if (cell.occupied)
                {
                    Destroy(cell.node.gameObject);
                    cell.node.Delete();
                }
            }

        }

        public void BuildItem()
        {
            
        }

        private void OnSetBuildItem(Node newBuildItem)
        {
            currentBuildItem = newBuildItem;
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
