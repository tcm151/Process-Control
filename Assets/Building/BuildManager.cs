using System;
using ProcessControl.Conveyors;
using ProcessControl.Tools;
using ProcessControl.Terrain;
using UnityEngine;
using UnityEngine.EventSystems;
using Grid = ProcessControl.Terrain.Grid;


namespace ProcessControl.Building
{
    public class BuildManager : MonoBehaviour
    {
        public Node selectedNodeType;

        private bool buildMode;
        new private Camera camera;
        
        public static Action<bool> OnBuildModeChanged;
        public static Action<Node> SetBuildItem;

        private Node firstNode, secondNode;
        private Node previousNode;

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
            
            if (!buildMode || !selectedNodeType || EventSystem.current.IsPointerOverGameObject()) return;

            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                var firstCell = Grid.GetCellPosition(camera.MouseWorldPosition2D());
                if (firstCell is null) Debug.Log("NO CELL FOUND!");
                else
                {
                    firstNode = (firstCell.occupied) ? firstCell.node : BuildNode(firstCell);
                    firstCell.node = firstNode;
                }
            }

            // if (Input.GetKey(KeyCode.Mouse0))
            // {
            //     var cell = Grid.GetCellPosition(camera.MouseWorldPosition2D());
            //     if (cell is null)  Debug.Log("NO CELL FOUND!");
            //     else
            //     {
            //         var node = (cell.occupied) ? cell.node : Factory.Spawn(selectedNodeType, cell.center);
            //         if (previousNode is { })
            //         {
            //             cell.node = node;
            //             node.Connect(previousNode);
            //             previousNode.Connect(node);
            //         }
            //         
            //         previousNode = cell.node;
            //     }
            // }
            
            if (Input.GetKeyUp(KeyCode.Mouse0))
            {
                var secondCell = Grid.GetCellPosition(camera.MouseWorldPosition2D());
                if (secondCell is null) Debug.Log("NO CELL FOUND!");
                else
                {
                    secondNode = (secondCell.occupied) ? secondCell.node : BuildNode(secondCell);
                    secondCell.node = secondNode;
                    
                    firstNode.AddConnection(secondNode);
                    secondNode.AddConnection(firstNode);
                }
            }

            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                var cell = Grid.GetCellPosition(camera.MouseWorldPosition2D());
                if (cell is null) Debug.Log("NO CELL FOUND!");
                else
                if (cell.occupied)
                {
                    Destroy(cell.node.gameObject);
                    cell.node.Delete();
                }
            }

        }

        public Node BuildNode(Grid.Cell cell) => Factory.Spawn("Nodes", selectedNodeType, cell.center);

        private void OnSetBuildItem(Node newBuildItem)
        {
            selectedNodeType = newBuildItem;
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
