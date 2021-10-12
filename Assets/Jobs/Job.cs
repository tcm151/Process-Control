using System;
using System.Collections.Generic;
using System.Linq;
using ProcessControl.Procedural;


namespace ProcessControl.Jobs
{
    public class Task
    {
        public Cell location;
        public Action action;

        public bool completed = false;
    }
    
    public class Job
    {
        public Cell origin;
        public List<Task> tasks;

        public bool Completed => tasks.TrueForAll(t => t.completed);
    }

    public interface IWorker
    {
        public void TakeJob(Job newJob);
    }
}