using System;
using System.Collections.Generic;
using System.Linq;
using ProcessControl.Procedural;


namespace ProcessControl.Jobs
{
    [Serializable] public class Job
    {
        // must do prerequisite before current job
        public Job prerequisite;
        
        public Cell destination;
        public Action action;
        
        public bool complete = false;
    }
    
    public interface IWorker
    {
        public void TakeJob(Job newJob);
    }
}