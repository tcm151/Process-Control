
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using ProcessControl.Jobs;
using ProcessControl.Tools;

public class AgentManager : MonoBehaviour
{
    public static Action<Job> QueueJob;
    
    private List<WorkerAgent> currentWorkers = new List<WorkerAgent>();
    private List<Job> openJobs = new List<Job>();

    private void Awake()
    {
        QueueJob += OnQueueJob;
        
        currentWorkers = FindObjectsOfType<WorkerAgent>().ToList();
    }

    public void OnQueueJob(Job newJob)
    {
        Debug.Log("Job added to queue...");
        openJobs.Add(newJob);
    }
    
    private void FixedUpdate()
    {
        if (openJobs.Count >= 1 && currentWorkers.Count >= 1)
        {
            var availableWorker = currentWorkers.FirstOrDefault(w => w.currentJob == null);
            if (availableWorker is null) return;
            availableWorker.AcceptJob(openJobs.TakeFirst());
        }
    }
}