using System;
using System.Collections;
using System.Collections.Generic;
using ProcessControl.Construction;
using ProcessControl.Procedural;
using ProcessControl.Tools;
using UnityEngine;


namespace ProcessControl.Pathfinding
{
    public class Agent : MonoBehaviour
    {
        public enum Movement { Idle, Moving }
        // public enum Task { DoingJob, AcceptingJob }
        
        // private Movement movement;
        
        // private Vector3 startPosition, endPosition;
        protected List<Vector3> currentPath = new List<Vector3>();

        private bool buildMode;
        
        private void Awake()
        {
            // movement = Movement.Idle;
            ConstructionManager.OnBuildModeChanged += isEnabled => buildMode = isEnabled;

        }

        // private void Update()
        // {
        //     if (buildMode) return;
        //     
        //     if (Input.GetKeyDown(KeyCode.Mouse1))
        //     {
        //         var startCell = ProceduralGrid.GetCellPosition(transform.position);
        //         startPosition = startCell.position;
        //         var endCell = ProceduralGrid.GetCellUnderMouse();
        //         endPosition = endCell.position;
        //
        //         currentPath?.Clear();
        //
        //         var newPath = AStar.FindPath(startPosition, endPosition);
        //         if (newPath is { }) currentPath = newPath;
        //     }
        // }

        private void FixedUpdate()
        {
            if (currentPath is { } && currentPath.Count >= 1)
            {
                var currentPosition = transform.position;
                if (currentPosition.DistanceTo(currentPath[0]) < 0.5f) currentPath.RemoveAt(0);
                // if (currentPosition == currentPath[0]) 
                if (currentPath.Count == 0)
                {
                    currentPath = null;
                    return;
                }
                
                currentPosition.MoveTowards(currentPath[0], 2.5f * Time.deltaTime);
                transform.up = -transform.position.DirectionTo(currentPath[0]);
                transform.position = currentPosition;
            }
        }

        private void OnDrawGizmos()
        {
            if (currentPath is null || currentPath.Count == 0) return;

            Gizmos.DrawLine(transform.position, currentPath[0]);
            for (int i = 0; i < currentPath.Count - 1; i++)
            {
                Gizmos.DrawLine(currentPath[i], currentPath[i+1]);
            }
        }
    }
}