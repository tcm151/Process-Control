﻿using System;
using System.Collections.Generic;
using System.Linq;
using ProcessControl.Construction;
using ProcessControl.Tools;
using UnityEngine;


namespace ProcessControl.Pathfinding
{
    public class Agent : MonoBehaviour
    {
        public float speed = 2.5f;
        
        public enum Movement { Idle, Moving }
        // public enum Task { DoingJob, AcceptingJob }
        
        // private Movement movement;
        
        // private Vector3 startPosition, endPosition;
        protected List<Vector3> currentPath = new List<Vector3>();
        
        public event Action onReachedDestination;

        public Vector3 position
        {
            get => transform.position;
            set => transform.position = value;
        }
        
        private bool buildMode;
        
        virtual protected void Awake()
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
                if (currentPosition.DistanceTo(currentPath[0]) < 0.2f) currentPath.RemoveAt(0);
                if (currentPath.Count == 0 || currentPosition.DistanceTo(currentPath.Last()) < 1.5f)
                {
                    onReachedDestination?.Invoke();
                    currentPath = null;
                    return;
                }
                
                currentPosition.MoveTowardsR(currentPath[0], speed * Time.deltaTime);
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