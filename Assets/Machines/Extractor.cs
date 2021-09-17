using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ProcessControl.Conveyors;
using ProcessControl.Tools;


namespace ProcessControl
{
    public class Extractor : Machine
    {
        [SerializeField] public Machine output;
        
        [Range(0, 64)] public float extractionSpeed;
        [SerializeField] public Resource extractionResource;
        [SerializeField] public int inventorySize = 64;
        [SerializeField] private List<Resource> inventory = new List<Resource>();


        override public bool Full => (inventory.Count >= inventorySize);
        
        public int ticks;
        public bool sleeping;
        
        private void FixedUpdate()
        {
            if (sleeping) return;
            
            if (++ticks % (64 / extractionSpeed) == 0)
            {
                if (Full) return;
                
                ticks = 0;
                if (!output || output.Full) inventory.Add(ExtractResource());
                else if (!output.Full) output.DepositResource(ExtractResource());
            }
        }

        protected Resource ExtractResource()
        {
            var resource = Factory.Spawn("Resources", extractionResource, Position);
            // do more stuff
            return resource;
        }

        override public void ConnectOutput(Machine node)
        {
            if (node == this) return;
            base.ConnectOutput(node);
            output = node;
        }

        override public void DepositResource(Resource resource) { }

        override public Resource WithdrawResource()
        {
            if (inventory.Count >= 1)
            {
                return inventory.TakeFirst();
            }
            else return null;
        }
        
        
    }
}
