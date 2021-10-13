using System;
using System.Collections.Generic;
using System.Linq;
using ProcessControl.Procedural;


namespace ProcessControl.Jobs
{
    public class Step
    {
        public Cell location;
        public Action action;
    
        public bool completed = false;
    }
    
    public class Job
    {
        public Cell origin;
        public List<Step> steps;

        public bool Completed => steps.TrueForAll(s => s.completed);
    }

    public interface IWorker
    {
        public void TakeJob(Job newJob);
    }
}