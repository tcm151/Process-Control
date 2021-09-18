using System;
using System.Collections.Generic;
using ProcessControl.Tools;
using ProcessControl.Graphs;
using UnityEngine;


namespace ProcessControl.Machines
{
    public class Machine : Node, IO<Conveyor>
    {
        protected const int TicksPerSecond = 64;
        
        //> MACHINE DATA CONTAINER
        [Serializable] new public class Data
        {
            public bool sleeping;
            public int ticks;
            public int sleepThreshold = 256;

            [Header("IO")]
            public Conveyor input;
            public bool inputEnabled = true;
            public Conveyor output;
            public bool outputEnabled = true;
           
            [Header("Inventory")]
            public int inventorySize = 8;
            public List<Resource> inventory = new List<Resource>();
        }
        [SerializeField] public Data machine;
        
        //> IO INTERFACE
        public Conveyor Input => machine.input;
        public Conveyor Output => machine.output;
        
        //> PROPERTIES
        virtual public bool Full => machine.inventory.Count >= InventorySize;
        virtual public bool Empty => machine.inventory.Count == 0;
        virtual public int InventorySize => machine.inventorySize;

        override public void Delete() => base.Delete();

        //> CONNECT INPUT
        public void ConnectInput(Conveyor input)
        {
            if (!ConnectEdge(input)) return;
            machine.input = input;
        }

        //> CONNECT OUTPUT
        virtual public void ConnectOutput(Conveyor output)
        {
            if (!ConnectEdge(output)) return;
            machine.output = output;
        }
        
        //> DEPOSIT RESOURCE
        virtual public void Deposit(Resource resource)
        {
            machine.inventory.Add(resource);
            resource.data.position = Position;
        }

        //> WITHDRAW RESOURCE
        virtual public Resource Withdraw() => machine.inventory.TakeFirst();
        
        //> FIXED CALCULATION INTERVAL
        virtual protected void FixedUpdate()
        {
            if (machine.sleeping) return;
            if (machine.ticks >= machine.sleepThreshold) machine.sleeping = true;
        }
    }
}
