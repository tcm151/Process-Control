using System;
using System.Collections.Generic;
using ProcessControl.Conveyors;
using UnityEngine;

namespace ProcessControl
{
    abstract public class Machine : Node
    {
        [Serializable] new public class Data
        {
            public Machine input;
            public Machine output;
           
            public List<Resource> inventory = new List<Resource>();
        }

        override protected void Awake()
        {
            base.Awake();

            OnAddConnection += UpdateConnections;
        }

        [SerializeField] protected Data machineData;
        
        protected int ticks;
        protected bool sleeping;

        private void UpdateConnections(Node node)
        {
            var machine = node.gameObject.GetComponent<Machine>();
            
            // if (machine.machineData.input)
        }
        
        virtual public void ConnectInput(Machine machine)
        {
            if (!AddConnection(machine)) return;
            machineData.input = machine;
        }

        virtual public void ConnectOutput(Machine machine)
        {
            if (!AddConnection(machine)) return;
            machineData.output = machine;
        }
        
        abstract public void DepositResource(Resource resource);
        abstract public Resource WithdrawResource();
    }
}
