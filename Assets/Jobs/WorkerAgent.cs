using System;
using ProcessControl.Pathfinding;


namespace ProcessControl.Jobs
{
    public class WorkerAgent : Agent, IWorker
    {
        private Job currentJob;
        public void TakeJob(Job newJob)
        {
            currentJob = newJob;
            currentPath = AStar.FindPath(transform.position, currentJob.destination.position);
        }

        private void Update()
        {
            
        }
    }
}