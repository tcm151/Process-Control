using System;
using ProcessControl.Pathfinding;
using UnityEngine;


namespace ProcessControl.Jobs
{
    public class Worker : Agent, IWorker
    {
        public Job currentJob {get; private set;}
        public void TakeJob(Job newJob)
        {
            currentJob = newJob;
            currentPath = AStar.FindPath(transform.position, currentJob.location);
        }

        public event Action onJobCompleted;

        override protected void Awake()
        {
            base.Awake();
            onReachedDestination += () =>
            {
                
            };
        }

        private async void CompleteJob()
        {
            Debug.Log("Job location reached.");
            Debug.Log("Starting job.");
            // await currentJob.action();
            currentJob.action();
            Debug.Log("Job complete.");
            currentJob.complete = true;
            onJobCompleted?.Invoke();
        }

        private void Update()
        {
            
        }
    }
}