using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using ProcessControl.Tools;


namespace ProcessControl.Pathfinding
{
    public class Agent : MonoBehaviour
    {
        [Header("Roaming")]
        public float roamingInterval = 2.5f;
        public float roamingDistance = 5f;
        internal CancellationTokenSource roamingCancel;
        
        [Header("Movement")]
        public float speed = 2.5f;
        virtual protected bool Idle => true;
        
        [Header("Path")]
        [SerializeField] protected List<Vector3> currentPath = new List<Vector3>();
        // protected Path<Vector3> currentPath;

        //> EVENTS
        public event Action onReachedDestination;

        public Vector3 position
        {
            get => transform.position;
            set => transform.position = value;
        }
        
        //> INITIALIZATION
        virtual protected void Awake()
        {
            roamingCancel = new CancellationTokenSource();
        }
        
        public void CancelAction()
        {
            // Debug.Log("Cancelling current action...");
            roamingCancel.Cancel();
            roamingCancel = new CancellationTokenSource();
        }
        
        //> ROAM WHEN ENTERING PLAYMODE
        virtual protected void Start()
        {
            if (!Idle) return;
            
            Roam();
            Debug.Log("Roam on play.");
        }
        
        //> ROAM AROUND RANDOMLY
        protected async void Roam()
        {
            // maybe remove
            if (!Idle)
            {
                Debug.Log("BUSY GUY");
                return;
            }
            
            var waitTime = 0f;
            while ((waitTime += Time.deltaTime) < roamingInterval)
            {
                if (!Idle || roamingCancel.IsCancellationRequested) return;
                await Task.Yield();
            }

            var currentPosition = transform.position;
            currentPath = await AStar.FindPath_Async(currentPosition, currentPosition + UnityEngine.Random.insideUnitCircle.ToVector3() * roamingDistance);
        }

        //> MOVE ALONG CURRENT PATH IF ONE EXISTS
        private void FixedUpdate()
        {
            if (currentPath is { } && currentPath.Count >= 1)
            {
                var currentPosition = transform.position;
                if (currentPosition.DistanceTo(currentPath[0]) < 0.25f) currentPath.RemoveAt(0);
                // if (currentPosition.DistanceTo(currentPath.currentPoint) < 0.25f) currentPath.NextPoint();
                
                // if (currentPosition.DistanceTo(currentPath.Destination) < 1.5f)
                if (currentPath.Count == 0 || currentPosition.DistanceTo(currentPath.Last()) < 1f)
                {
                    currentPath = null;
                    onReachedDestination?.Invoke();
                }
                else
                {
                    currentPosition.MoveTowardsR(currentPath[0], speed * Time.deltaTime);
                    transform.up = -transform.position.DirectionTo(currentPath[0]);
                    transform.position = currentPosition;
                }
                
            }
        }

        //> DRAW PATH
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
        
        //> CANCEL ROAMING ON QUIT
        private void OnApplicationQuit() => roamingCancel.Cancel();
    }
}