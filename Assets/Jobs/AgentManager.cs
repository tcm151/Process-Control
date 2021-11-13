
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using ProcessControl.Tools;


namespace ProcessControl.Jobs
{
    public class AgentManager : MonoBehaviour
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
                    openWorkers.Add(worker);
                    busyWorkers.Remove(worker);

                    var job = worker.currentJob;
                    takenJobs.Remove(job);
                    completedJobs.Add(job);
                    worker.currentJob = null;
                };
            });
        }
        
        //> ASSIGN JOBS
        private void FixedUpdate()
        {
            // no jobs or no workers
            if (openJobs.Count < 1 || openWorkers.Count < 1) return;
            
            // assign the job closest to the first available worker
            var worker = openWorkers.TakeFirst();
            var closestJob = openJobs.OrderBy(j => Vector3.Distance(worker.position, j.location)).First();
            // var closestJob = openJobs.OrderBy(j => Vector3.Distance(worker.position, j.location)).First(j => j.prerequisite is {complete: true} || j.prerequisite is null);
            openJobs.Remove(closestJob);
            worker.TakeJob(closestJob);
            takenJobs.Add(closestJob);
            busyWorkers.Add(worker);
        }
    }
}

