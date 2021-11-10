
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
        
        [SerializeField] private List<Job> openJobs = new List<Job>();
        [SerializeField] private List<Job> takenJobs = new List<Job>();
        [SerializeField] private List<Job> completedJobs = new List<Job>();
        [SerializeField] private List<Worker> openWorkers = new List<Worker>();
        [SerializeField] private List<Worker> busyWorkers = new List<Worker>();

        private void Awake()
        {
            QueueJob += (job) => openJobs.Add(job);
            QueueJobs += (jobList) => jobList.ForEach(j => openJobs.Add(j));
            
            openWorkers = FindObjectsOfType<Worker>().ToList();
            openWorkers.ForEach(worker => 
            {
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
        
        private void FixedUpdate()
        {
            if (openJobs.Count >= 1 && openWorkers.Count >= 1)
            {
                var worker = openWorkers.TakeFirst();

                var closestJob = openJobs.OrderBy(j => Vector3.Distance(worker.position, j.location)).First();
                openJobs.Remove(closestJob);
                worker.TakeJob(closestJob);
                takenJobs.Add(closestJob);
                
                busyWorkers.Add(worker);
            }
        }
    }
}

