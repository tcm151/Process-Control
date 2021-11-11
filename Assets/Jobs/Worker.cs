using System;
using System.Threading;
using System.Threading.Tasks;
using ProcessControl.Pathfinding;
using ProcessControl.Tools;
using UnityEngine;
using Random = UnityEngine.Random;


namespace ProcessControl.Jobs
{
    public class Worker : Agent, IWorker
    {
        internal Job currentJob;
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

        private void Start() => Roam();

        public async void CompleteJob()
        {
            if (currentJob is null)
            { 
                Roam();
                return;
            }
            
            await currentJob.order();
            currentJob.complete = true;
            onJobCompleted?.Invoke();
        }

        private async void Roam()
        {
            var time = 0f;
            while ((time += Time.deltaTime) < 2.5f) await Task.Yield();
            if (currentJob is {}) return;
            currentPath = AStar.FindPath(transform.position, transform.position + Random.insideUnitCircle.ToVector3() * 5f);
        }

        private void OnDestroy() => currentJob = null;
    }
}