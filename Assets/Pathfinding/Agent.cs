using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using ProcessControl.Tools;
using ProcessControl.Industry;


namespace ProcessControl.Pathfinding
{
    public class Agent : MonoBehaviour
    {
        public float speed = 2.5f;
        
        // public enum Movement { Idle, Moving }
        // public enum Task { DoingJob, AcceptingJob }
        
        protected List<Vector3> currentPath = new List<Vector3>();
        // protected Path<Vector3> currentPath;

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
            ConstructionManager.OnBuildModeChanged += (isEnabled) => buildMode = isEnabled;

        }

        private void FixedUpdate()
        {
            if (currentPath is { } && currentPath.Count >= 1)
            // if (currentPath is { })
            {
                var currentPosition = transform.position;
                if (currentPosition.DistanceTo(currentPath[0]) < 0.25f) currentPath.RemoveAt(0);
                // if (currentPosition.DistanceTo(currentPath.currentPoint) < 0.25f) currentPath.NextPoint();
                if (currentPath.Count == 0 || currentPosition.DistanceTo(currentPath.Last()) < 1.5f)
                // if (currentPosition.DistanceTo(currentPath.Destination) < 1.5f)
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
            // if (currentPath is null || currentPath.completed) return;

            Gizmos.DrawLine(transform.position, currentPath[0]);
            for (int i = 0; i < currentPath.Count - 1; i++)
            {
                Gizmos.DrawLine(currentPath[i], currentPath[i+1]);
            }
        }
    }
}