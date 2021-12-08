using System;
using System.Collections.Generic;


namespace ProcessControl.Jobs
{
    [Serializable] public class Job
    {
        public static Job EmptyJob => new Job
        {
            description = "Empty Job.",
            orders = { Order.EmptyOrder },
            complete = true,
        };
        
        public string description = "no job.";

        public Job prerequisite;
        public Worker activeWorker;
        // public List<Worker> activeWorkers;
        
        public List<Order> orders = new List<Order>();

        public bool complete;
    }
}