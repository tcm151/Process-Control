using System;
using System.Collections.Generic;
using System.Linq;
using ProcessControl.Procedural;
using UnityEngine;
using UnityEngine.Serialization;


namespace ProcessControl.Jobs
{
    [Serializable] public class Job
    {
        // must do prerequisite before current job
        public Job prerequisite;
        
        // public Cell destination;
        [FormerlySerializedAs("destination")] public Vector3 location;
        public Action action;
        
        public bool complete;
    }
    
    public interface IWorker
    {
        public void TakeJob(Job newJob);
    }
}