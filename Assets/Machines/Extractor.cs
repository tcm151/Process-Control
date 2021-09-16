using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ProcessControl.Conveyors;

namespace ProcessControl
{
    public class Extractor : Machine
    {
        [SerializeField] private List<Resource> inventory = new List<Resource>();

        public float extractionSpeed;

        public Node output;

        private int ticks;
        
        private void FixedUpdate()
        {
            ticks++;

            if (ticks > extractionSpeed * 64)
            {
                ticks = 0;
                if (!output) inventory.Add(new Resource());
                output.DepositResource(new Resource());
            }
        }

        override public void ConnectOutput(Node node)
        {
            base.ConnectOutput(node);
            output = node;
        }

        override public Resource WithdrawResource()
        {
            if (inventory.Count >= 1)
            {
                var resource = inventory[0];
                inventory.RemoveAt(0);
                return resource;
            }
            else return null;
        }
        
        
    }
}
