using System;
using System.Collections.Generic;
using UnityEngine;
using ProcessControl.Tools;


namespace ProcessControl.Machines
{
    abstract public class Machine : Node, IO
    {
        public const int TicksPerSecond = 64;
        public const int ItemsPerSecond = 1;
        
        [Serializable] new public class Data
        {
            public int ticks;
            public bool sleeping;
            
            public Edge input;
            public Edge output;
           
            public List<Resource> inventory = new List<Resource>();
        }

        virtual public int InventorySize => 0;
        
        [SerializeField] public Data machine;

        override protected void Awake()
        {
            base.Awake();
            // do other stuff
            machine = new Data();
        }

        virtual protected void FixedUpdate()
        {
            if (machine.sleeping) return;
            if (machine.ticks >= 512) machine.sleeping = true;
        }
        
        public void ConnectInput(Edge input)
        {
            if (!ConnectEdge(input)) return;
            machine.input = input;
        }

        public void ConnectOutput(Edge output)
        {
            if (!ConnectEdge(output)) return;
            machine.output = output;
        }
        
        override public void Deposit(Resource resource) => machine.inventory.Add(resource);
        override public Resource Withdraw() => machine.inventory.TakeFirst();
    }
}
