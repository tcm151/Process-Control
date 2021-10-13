using System;
using System.Threading.Tasks;
using ProcessControl.Procedural;
using ProcessControl.Tools;
using UnityEngine;


namespace ProcessControl.Jobs
{
    public class Worker : MonoBehaviour, IWorker
    {
        private Job currentJob;
        public void TakeJob(Job newJob) => currentJob = newJob;

        
        
    }
}