using System;
using System.Collections.Generic;
using System.Linq;
using ProcessControl.Tools;
using UnityEngine;


namespace ProcessControl.Conveyors
{
    public class Conveyor : Machine
    {
        public const int TicksPerSecond = 64;
        public const int ItemsPerSecond = 1;
        
        
        
        override public bool Full => (machineData.inventory.Count >= MaxItems);
        public int MaxItems => nodeData.connections.Sum(n => (int) DistanceBetween(this, n));

        private void Awake()
        {
            // other stuff
            machineData = new Data();
        }

        private void FixedUpdate()
        {
            
            if (++ticks % (TicksPerSecond / ItemsPerSecond) == 0)
            {
                if (Full) return;
                ticks = 0;
                
                if (machineData.output && machineData.inventory.Count >= 1)
                {
                    if (machineData.output.Full) return;
                    machineData.output.DepositResource(WithdrawResource());
                }
                
                // if (conveyorData.input)
                // {
                //     var resource = conveyorData.input.WithdrawResource();
                //     if (resource is { }) conveyorData.inventory.Add(resource);
                // } 
                // if (conveyorData.input && NumberOfResources < MaxItems) conveyorData.input.WithdrawResource();
            }

        }

        // override public void ConnectInput(Machine node)
        // {
        //     base.ConnectInput(node);
        //     machineData.input = node as Machine;
        // }
        //
        // override public void ConnectOutput(Machine node)
        // {
        //     base.ConnectInput(node);
        //     machineData.output = node as Machine;
        // }

        override public void DepositResource(Resource resource) => machineData.inventory.Add(resource);
        override public Resource WithdrawResource() => machineData.inventory.TakeFirst();
    }
}
