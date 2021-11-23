using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ProcessControl.Industry;
using UnityEngine;


namespace ProcessControl.Jobs
{
    [Serializable] public class Order
    {
        public string description = "untitled job.";
        
        // must do prerequisite before current job
        public Order prerequisite;


        public Vector3 location;
        public Func<Task> order;
        public List<ItemAmount> requiredItems = new List<ItemAmount>();
        
        public bool complete;
    }

    public class Job
    {
        private List<Order> orders;
    }

    // [Serializable] public class ConstructionJob : Order
    // {
    //     public ConstructionJob(Vector3 position, IBuildable buildable, float constructionTime)
    //     {
    //         location = position;
    //         order = () => buildable.Build(constructionTime);
    //     }
    // }
    //
    // [Serializable] public class DeliveryJob : Order
    // {
    //     public DeliveryJob(IInventory recipient, ItemAmount itemAmount, Vector3 position)
    //     {
    //         location = position;
    //         order = () =>
    //         {
    //             recipient.Deposit(itemAmount);
    //             return Task.CompletedTask;
    //         };
    //     }
    // }
    
    public interface IWorker
    {
        public void TakeJob(Order newOrder);
        public void CompleteJob();
    }
}