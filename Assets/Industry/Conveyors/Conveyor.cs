using System;
using System.Collections.Generic;
using ProcessControl.Tools;
using ProcessControl.Graphs;
using UnityEngine;


namespace ProcessControl.Machines
{
    public class Conveyor : Edge, IO<Machine>
    {
        private const int TicksPerSecond = 64;
        

        //> CONVEYOR DATA CONTAINER
        [Serializable] new public class Data
        {
            public bool sleeping;
            public int ticks;
            public int sleepThreshold = 256;
            [Space(8)]
            public int itemsPerSecond = 8;

            [Header("IO")]
            public Machine input;
            public Machine output;
            public float distanceBetweenIO;

            [Header("Inventory")]
            public int inventorySize = 8;
            public List<Resource> inventory = new List<Resource>();
        }
        [SerializeField] private Data conveyor;

        //> IO INTERFACE
        public Machine Input => conveyor.input;
        public Machine Output => conveyor.output;
        
        //> PROPERTIES
        public bool Full => conveyor.inventory.Count >= InventorySize;
        public bool Empty => conveyor.inventory.Count == 0;

        //> MAXIMUM INVENTORY ALLOWED
        public int InventorySize { get
        {
            if (conveyor.input is null || conveyor.output is null) return 0;
            return (int) Node.DistanceBetween(conveyor.input, conveyor.output);
        }}

        //> ADD AND REMOVE RESOURCES
        public void Deposit(Resource resource) => conveyor.inventory.Add(resource);
        public Resource Withdraw() => conveyor.inventory.TakeFirst();

        //> CONNECT INPUT
        public void ConnectInput(Machine input)
        {
            conveyor.input = input;
            onConnection?.Invoke();
        }

        //> CONNECT OUTPUT
        public void ConnectOutput(Machine output)
        {
            conveyor.output = output;
            onConnection?.Invoke();
        }
        
        //> EVENTS
        public event Action onConnection;

        //> EVENT REGISTRATION
        private void Awake() => onConnection += ManageConnection;
        private void ManageConnection()
        {
            if (conveyor.input is null || conveyor.output is null) conveyor.distanceBetweenIO = 0;
            else conveyor.distanceBetweenIO = Node.DistanceBetween(conveyor.input, conveyor.output);
        }

        override public void Delete()
        {
            conveyor.input.machine.output = null;
            conveyor.output.machine.input = null;
            conveyor.inventory.ForEach(Destroy);
            Destroy(this.gameObject);
        }

        //> FIXED CALCULATION INTERVAL
        protected void FixedUpdate()
        {
            // if (conveyor.sleeping) return;
            // if (conveyor.ticks >= conveyor.sleepThreshold) conveyor.sleeping = true;
            
            //> EVERY INTERVAL PULL FROM INPUT IF CAPABLE
            if (++conveyor.ticks % (TicksPerSecond / conveyor.itemsPerSecond) == 0)
            {
                if (!Full && !conveyor.input.Empty)
                {
                    conveyor.ticks = 0;
                    Deposit(conveyor.input.Withdraw());
                }
            }

            //> MOVE RESOURCES ALONG CONVEYOR PATH
            for (int i = 0; i < conveyor.inventory.Count; i++)
            {
                var resource = conveyor.inventory[i];
                resource.data.ticks++;

                // float percentage = resource.data.ticks / (conveyor.distanceBetweenIO * TicksPerSecond / conveyor.itemsPerSecond);
                
                float indexPercentage = (InventorySize - i) / (float) InventorySize;
                var indexPosition = conveyor.input.Position + conveyor.input.DirectionTo(conveyor.output) * (conveyor.distanceBetweenIO * indexPercentage);
                resource.data.position.MoveTowards(indexPosition, (float) conveyor.itemsPerSecond / TicksPerSecond);
            }


            if (Empty) return;
            if (conveyor.inventory[0].data.ticks > (int)conveyor.distanceBetweenIO * TicksPerSecond / conveyor.itemsPerSecond)
            {
                if (conveyor.output && !conveyor.output.Full && conveyor.inventory.Count >= 1)
                {
                    conveyor.inventory[0].data.ticks = 0;
                    conveyor.output.Deposit(Withdraw());
                }
            }
        }

        private void OnDrawGizmos()
        {
            if (!conveyor.input || !conveyor.output) return;
            
            Gizmos.color = Color.green;
            Gizmos.DrawLine(conveyor.input.Position, conveyor.output.Position);
        }
    }
}
