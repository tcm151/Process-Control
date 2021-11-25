
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using ProcessControl.Tools;


namespace ProcessControl.Jobs
{
    public class JobManager : MonoBehaviour
    {
        public static Action<Job> QueueJob;
        public static Action<List<Job>> QueueJobs;
        
        [Header("Jobs")]
        [SerializeField] private List<Job> openJobs = new List<Job>();
        [SerializeField] private List<Job> takenJobs = new List<Job>();
        [SerializeField] private List<Job> completedJobs = new List<Job>();
        
        [Header("Workers")]
        [SerializeField] private List<Worker> openWorkers = new List<Worker>();
        [SerializeField] private List<Worker> busyWorkers = new List<Worker>();

        //> INITIALIZATION
        private void Awake()
        {
            // program events
            QueueJob += (job) => openJobs.Add(job);
            QueueJobs += (jobList) => jobList.ForEach(j => openJobs.Add(j));
            
            // collect workers
            openWorkers = FindObjectsOfType<Worker>().ToList();
            openWorkers.ForEach(worker => 
            {
                // setup protocol when workers finish their job
                worker.onJobCompleted += () => 
                {
                    busyWorkers.Remove(worker);
                    openWorkers.Add(worker);

                    var job = worker.currentJob;
                    takenJobs.Remove(job);
                    completedJobs.Add(job);
                    job.activeWorker = null;
                    worker.currentJob = null;
                    worker.currentOrder = null;
                };
            });
        }
        
        //> ASSIGN JOBS
        private void FixedUpdate()
        {
            // no jobs or no workers
            if (openJobs.Count < 1 || openWorkers.Count < 1) return;
            
            // assign the job closest to the first available worker
            var worker = openWorkers.TakeAndRemoveFirst();
            var closestJob = openJobs.OrderBy(j => Vector3.Distance(worker.position, j.orders.First(o => !o.complete).location)).First();
            worker.TakeJob(closestJob);
            busyWorkers.Add(worker);
            openJobs.Remove(closestJob);
            takenJobs.Add(closestJob);
        }
    }
}

