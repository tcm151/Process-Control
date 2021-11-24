using System;
using System.Collections.Generic;


namespace ProcessControl.Jobs
{
    [Serializable] public class Job
    {
        public Worker activeWorker;
        
        public List<Order> orders = new List<Order>();

        public bool complete;
    }
}