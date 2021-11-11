using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ProcessControl.Industry;
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

    [Serializable] public class ConstructionJob : Job
    {
        public ConstructionJob(Vector3 position, IBuildable buildable, float constructionTime)
        {
            location = position;
            order = () => buildable.Build(constructionTime);
        }
    }

    [Serializable] public class DeliveryJob : Job
    {
        public DeliveryJob(IInventory recipient, ItemAmount itemAmount, Vector3 position)
        {
            location = position;
            order = () =>
            {
                recipient.Deposit(itemAmount);
                return Task.CompletedTask;
            };
        }
    }
    
    public interface IWorker
    {
        public void TakeJob(Job newJob);
        public void CompleteJob();
    }
}