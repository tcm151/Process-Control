using System;
using System.Collections.Generic;
using ProcessControl.Machines;
using UnityEngine;


namespace ProcessControl.Conveyors
{
    public class Conveyor : Node, IO
    {
        public const int TicksPerSecond = 64;
        public const int ItemsPerSecond = 1;
        
        [Serializable] new public class Data
        {
            public Node inputNode;
            public Node outputNode;
           
            public int ticks;
            public List<Resource> resources = new List<Resource>();
        }
        
        [SerializeField] private Data conveyorData;
        
        public int NumberOfResources => conveyorData.resources.Count;
        
        public int MaxItems
            => (conveyorData.inputNode && conveyorData.outputNode)
                ? (int) DistanceBetween(conveyorData.inputNode, conveyorData.outputNode)
                    : 0;


        private void Awake() => conveyorData = new Data
        {
            ticks = 0,
        };

        private void FixedUpdate()
        {
            conveyorData.ticks++;
            
            if (conveyorData.ticks >= TicksPerSecond / ItemsPerSecond)
            {
                conveyorData.ticks = 0;
                if (conveyorData.outputNode && conveyorData.resources.Count >= 1)
                {
                    conveyorData.outputNode.DepositResource(WithdrawResource());
                }
                // if (conveyorData.inputNode)
                // {
                //     var resource = conveyorData.inputNode.WithdrawResource();
                //     if (resource is { }) conveyorData.resources.Add(resource);
                // } 
                // if (conveyorData.inputNode && NumberOfResources < MaxItems) conveyorData.inputNode.WithdrawResource();
            }

        }

        override public void ConnectInput(Node node)
        {
            base.ConnectInput(node);
            conveyorData.inputNode = node;
        }

        override public void ConnectOutput(Node node)
        {
            base.ConnectInput(node);
            conveyorData.outputNode = node;
        }

        override public void DepositResource(Resource resource) => conveyorData.resources.Add(resource);
        override public Resource WithdrawResource()
        {
            if (conveyorData.resources.Count >= 1)
            {
                var resource = conveyorData.resources[0];
                conveyorData.resources.RemoveAt(0);
                return resource;
            }
            else return null;
        }
    }

    [Serializable] public class Resource { }
}
