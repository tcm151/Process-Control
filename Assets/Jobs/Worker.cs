using System;
using System.Threading.Tasks;
using ProcessControl.Pathfinding;
using ProcessControl.Tools;
using UnityEngine;
using Random = UnityEngine.Random;


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

        // private void Start()
        // {
        //     Roam();
        // }

        private async void CompleteJob()
        {
            // if (currentJob is null)
            // {
            //     await Task.Delay(2000);
            //     // Roam();
            //     return;
            // }
            
            await currentJob.order();
            currentJob.complete = true;
            currentJob = null;
            onJobCompleted?.Invoke();
            // if (currentJob is null) Roam();
        }

        // private void Roam()
        // {
        //     currentPath = AStar.FindPath(transform.position, transform.position + Random.insideUnitCircle.ToVector3() * 5f);
        // }
    }
}