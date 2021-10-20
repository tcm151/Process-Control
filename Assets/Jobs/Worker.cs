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
            onReachedDestination += CompleteJob;
        }

        private async void CompleteJob()
        {
            await currentJob.order();
            currentJob.complete = true;
            onJobCompleted?.Invoke();
        }
    }
}