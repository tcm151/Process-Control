using System;
using System.Threading.Tasks;
using UnityEngine;


namespace ProcessControl.Jobs
{
    [Serializable] public class Order
    {
        public static Order EmptyOrder => new Order
        {
            description = "Empty Order.",
            complete = true,
        };
        
        public string description = "no job.";
        
        public Vector3 location;
        public Func<Task> action;
        
        public bool complete;
    }
}