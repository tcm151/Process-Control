using System;
using System.Collections.Generic;
using System.Linq;
using ProcessControl.Procedural;


namespace ProcessControl.Jobs
{
    public class Job
    {
        public Cell destination;
        public Action action;
        public bool complete = false;
    }

    public interface IWorker
    {
        public void TakeJob(Job newJob);
    }
}