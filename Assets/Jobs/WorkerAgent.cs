using System;
using ProcessControl.Pathfinding;


namespace ProcessControl.Jobs
{
    public class WorkerAgent : Agent, IWorker
    {
        public Job currentJob {get; private set;}
        public void TakeJob(Job newJob)
        {
            currentJob = newJob;
            currentPath = AStar.FindPath(transform.position, currentJob.destination.position);
        }

        public event Action onJobCompleted;

        override protected void Awake()
        {
            base.Awake();
            onReachedDestination += () =>
            {
                
                currentJob.action();
                currentJob.complete = true;
                onJobCompleted?.Invoke();
            };
        }

        private void Update()
        {
            
        }
    }
}