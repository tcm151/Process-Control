using System;
using System.Threading.Tasks;
using UnityEngine;


namespace ProcessControl.Jobs
{
    [Serializable] public class Job
    {
        // must do prerequisite before current job
        public Job prerequisite;
        
        public Vector3 location;
        public Func<Task> order;
        
        public bool complete;
    }
    
    public interface IWorker
    {
        public void TakeJob(Job newJob);
        public void CompleteJob();
    }
}