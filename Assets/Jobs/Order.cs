using System;
using System.Threading.Tasks;
using UnityEngine;


namespace ProcessControl.Jobs
{
    [Serializable] public class Order
    {
        public string description = "untitled job.";
        
        // must do prerequisite before current job
        public Order prerequisite;


        public Vector3 location;
        public Func<Task> action;
        // public List<ItemAmount> requiredItems = new List<ItemAmount>();
        
        public bool complete;
    }
}