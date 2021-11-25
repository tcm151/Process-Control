using System;
using System.Collections.Generic;


namespace ProcessControl.Jobs
{
    [Serializable] public class Job
    {
        public string description = "no job.";
        
        public Worker activeWorker;
        // public List<Worker> activeWorkers;
        public List<Order> orders = new List<Order>();

        public bool complete;
    }
}