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
        [SerializeField] internal Data conveyor;

        new private SpriteRenderer renderer;
        public void SetLength(float size) => renderer.size = new Vector2(size, 1);

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
            return Mathf.CeilToInt(conveyor.distanceBetweenIO) * 2;
        }}

        //> ADD AND REMOVE RESOURCES
        public void Deposit(Resource resource) => conveyor.inventory.Add(resource);
        public Resource Withdraw() => conveyor.inventory.TakeFirst();
        
        //> EVENTS
        public event Action onConnection;

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


        //> INITIALIZATION
        private void Awake()
        {
            renderer = GetComponent<SpriteRenderer>();
            onConnection += ManageConnection;
        }

        //> MODIFY PROPERTIES ON CONNECTION
        private void ManageConnection()
        {
            if (conveyor.input is null || conveyor.output is null) conveyor.distanceBetweenIO = 0;
            else conveyor.distanceBetweenIO = Node.DistanceBetween(conveyor.input, conveyor.output);
        }

        //> DELETE CONVEYOR AND CLEAN UP
        override public void Delete()
        {
            conveyor.input.DisconnectOutput(this);
            conveyor.output.DisconnectInput(this);
            conveyor.inventory.ForEach(r => Destroy(r.gameObject));
            // Destroy(this.gameObject);
        }

        //> FIXED CALCULATION INTERVAL
        protected void FixedUpdate()
        {
            // if (conveyor.sleeping) return;
            // if (conveyor.ticks >= conveyor.sleepThreshold) conveyor.sleeping = true;
            
            //> EVERY INTERVAL PULL FROM INPUT IF CAPABLE
            if (++conveyor.ticks > (TicksPerSecond / (conveyor.itemsPerSecond * 2)))
            {
                if (!Full && !conveyor.input.Empty)
                {
                    conveyor.ticks = 0;
                    var resource = conveyor.input.Withdraw();
                    if (resource is { }) Deposit(resource);
                }
            }

            //> MOVE RESOURCES ALONG CONVEYOR PATH
            for (int i = 0; i < conveyor.inventory.Count; i++)
            {
                var resource = conveyor.inventory[i];
                resource.data.ticks++;
                
                var indexPercentage = conveyor.distanceBetweenIO * ((InventorySize - i) / (float) InventorySize);
                var indexPosition = conveyor.input.Position + conveyor.input.DirectionTo(conveyor.output) * indexPercentage;
                resource.data.position.MoveTowards(indexPosition, (float) conveyor.itemsPerSecond / TicksPerSecond);
            }


            //> DEPOSIT FIRST ITEM IN INVENTORY
            if (Empty) return;
            if (conveyor.inventory[0].data.ticks > conveyor.distanceBetweenIO * TicksPerSecond / conveyor.itemsPerSecond)
            {
                if (conveyor.output && !conveyor.output.Full && conveyor.inventory.Count >= 1)
                {
                    if (conveyor.output.machine.currentInput != this) return;
                    conveyor.inventory[0].data.ticks = 0;
                    conveyor.output.Deposit(Withdraw());
                }
            }
        }
    }
}
