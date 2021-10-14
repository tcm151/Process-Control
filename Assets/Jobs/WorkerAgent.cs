using System;
using ProcessControl.Pathfinding;


namespace ProcessControl.Jobs
{
    public class WorkerAgent : Agent, IWorker
    {
        public Job currentJob {get; private set;}
        public void AcceptJob(Job newJob)
        {
            currentJob = newJob;
            currentPath = AStar.FindPath(transform.position, currentJob.destination.position);
        }

        override protected void Awake()
        {
            base.Awake();
            onReachedDestination += () => currentJob = null;
        }

        private void Update()
        {
            
        }
    }
}