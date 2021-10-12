using System;
using System.Collections.Generic;
using ProcessControl.Procedural;
using ProcessControl.Tools;
using UnityEngine;


namespace ProcessControl.Pathfinding
{
    public class Agent : MonoBehaviour
    {
        private Vector3 startPosition, endPosition;
        private List<Vector3> currentPath;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                var startCell = ProceduralGrid.GetCellPosition(transform.position);
                startPosition = startCell.position;
                var endCell = ProceduralGrid.GetCellUnderMouse();
                endPosition = endCell.position;

                currentPath = AStar.FindPath(ProceduralGrid.GetChunks()[0].cells, startPosition, endPosition);
            }
        }

        private void FixedUpdate()
        {
            if (currentPath is null || currentPath.Count == 0) return;

            var currentPosition = transform.position;
            if (currentPosition == currentPath[0]) currentPath.RemoveAt(0);
            if (currentPath.Count == 0) return;
            currentPosition.MoveTowards(currentPath[0], 2.5f * Time.deltaTime);

            transform.position = currentPosition;
        }

        private void OnDrawGizmos()
        {
            if (currentPath is null || currentPath.Count == 0) return;

            for (int i = 0; i < currentPath.Count - 1; i++)
            {
                Gizmos.DrawLine(currentPath[i], currentPath[i+1]);
            }
        }
    }
}