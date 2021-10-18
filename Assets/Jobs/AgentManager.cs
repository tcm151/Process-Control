
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using ProcessControl.Jobs;
using ProcessControl.Tools;

public class AgentManager : MonoBehaviour
{
    public static Action<Job> QueueJob;
    public static Action<List<Job>> QueueJobs;

    [SerializeField] private List<WorkerAgent> busyWorkers = new List<WorkerAgent>();
    [SerializeField] private List<WorkerAgent> openWorkers = new List<WorkerAgent>();
    [SerializeField] private List<Job> openJobs = new List<Job>();

    private void Awake()
    {
        QueueJob += (job) =>
        {
            Debug.Log("Queuing new job...");
            openJobs.Add(job);
        };
        QueueJobs += jobList => jobList.ForEach(j => openJobs.Add(j));
        
        openWorkers = FindObjectsOfType<WorkerAgent>().ToList();
        openWorkers.ForEach(w =>
        {
            w.onJobCompleted += () =>
            {
                busyWorkers.Remove(w);
                openWorkers.Add(w);
            };
        });
    }
    
    private void FixedUpdate()
    {
        
        if (openJobs.Count >= 1 && openWorkers.Count >= 1)
        {
            //@ TODO get the closest worker by distance
            
            var worker = openWorkers.TakeFirst();
            worker.TakeJob(openJobs.TakeFirst());
            busyWorkers.Add(worker);
        }
    }
}