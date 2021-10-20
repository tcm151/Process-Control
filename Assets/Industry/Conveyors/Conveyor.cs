using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ProcessControl.Tools;
using ProcessControl.Graphs;
using ProcessControl.Industry;
using ProcessControl.Industry.Resources;
using UnityEngine.Serialization;
#pragma warning disable 108,114


namespace ProcessControl.Industry.Conveyors
{
    [SelectionBase]
    public class Conveyor : Edge
    {
        //> CONVEYOR DATA CONTAINER
        [Serializable] public class Data
        {
            public bool enabled;
            public bool sleeping;
            public int ticks;
            public int sleepThreshold = 256;
            
            [Header("Conveyor Speed")]
            public int itemsPerMinute = 8;

            [Header("IO")]
            public Node input;
            public Node output;
            public float distanceBetweenIO;

            [Header("Inventory")]
            public int inventorySize = 8;
            public List<Container> inventory = new List<Container>();
        }
        [SerializeField] internal Data conveyor;

        private SpriteRenderer renderer;
        public void SetLength(float size) => renderer.size = new Vector2(size, 1);

        override public float Length { get
        {
            if (conveyor.input is null || conveyor.output is null) return 0;
            return Node.DistanceBetween(conveyor.input, conveyor.output);
        }}

        //> ADD AND REMOVE RESOURCES
        override public void Deposit(Container container) => conveyor.inventory.Add(container);
        override public bool CanDeposit
            => conveyor.inventory.Count == 0
            || conveyor.inventory.Count < conveyor.inventorySize
            && conveyor.inventory.Count >= 1
            && conveyor.inventory.Last().ticks >= 2 * TicksPerSecond / conveyor.itemsPerMinute;
        
        override public Container Withdraw() => conveyor.inventory.TakeFirst();
        override public bool CanWithdraw
            => conveyor.inventory.Count >= 1
             && conveyor.inventory[0].ticks > conveyor.distanceBetweenIO * TicksPerSecond / conveyor.itemsPerMinute;
        
        override public IO Input => conveyor.input;
        override public IO Output => conveyor.output;

        public void Build()
        {
            conveyor.enabled = true;
        }

        //> CONNECT INPUT
        override public bool ConnectInput(IO input)
        {
            if (conveyor.input == input as Node) return false;
            conveyor.input = input as Node;
            ManageConnection();
            return true;
        }
        override public bool DisconnectInput(IO input)
        {
            if (conveyor.input != input as Node) return false;
            conveyor.input = null;
            ManageConnection();
            return true;
        }

        //> CONNECT OUTPUT
        override public bool ConnectOutput(IO output)
        {
            if (conveyor.output == output as Node) return false;
            conveyor.output = output as Node;
            ManageConnection();
            return true;
        }
        override public bool DisconnectOutput(IO output)
        {
            if (conveyor.output != output as Node) return false;
            conveyor.output = null;
            ManageConnection();
            return true;
        }

        //> INITIALIZATION
        private void Awake()
        {
            renderer = GetComponent<SpriteRenderer>();
        }

        //> MODIFY PROPERTIES ON CONNECTION
        private void ManageConnection()
        {
            if (conveyor.input is null || conveyor.output is null) conveyor.distanceBetweenIO = 0;
            else conveyor.distanceBetweenIO = Node.DistanceBetween(conveyor.input, conveyor.output);
            
            SetLength(conveyor.distanceBetweenIO);

            if (conveyor.input is { } && conveyor.output is { })
            {
                var direction = conveyor.input.DirectionTo(conveyor.output);
                var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            }
            
            conveyor.inventorySize = Mathf.CeilToInt(conveyor.distanceBetweenIO) * 2;
        }

        //> FIXED CALCULATION INTERVAL
        protected void FixedUpdate()
        {
            // if (!conveyor.enabled) return;
            
            //> EVERY INTERVAL PULL FROM INPUT IF CAPABLE
            if (++conveyor.ticks > (TicksPerMinute / (conveyor.itemsPerMinute * 2)))
            {
                if (CanDeposit && conveyor.input.CanWithdraw && conveyor.input.Output as Conveyor == this)
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
                
                var indexPercentage = conveyor.distanceBetweenIO * ((conveyor.inventorySize - i) / (float) conveyor.inventorySize);
                var indexPosition = conveyor.input.Position + conveyor.input.DirectionTo(conveyor.output) * indexPercentage;

                if (resource.position != indexPosition) resource.ticks++;
                
                resource.position = Vector3.MoveTowards(resource.position, indexPosition, conveyor.itemsPerMinute / (float) (TicksPerMinute));
            }


            //> DEPOSIT FIRST ITEM IN OUTPUT
            if (!CanWithdraw || !conveyor.output.CanDeposit || conveyor.output.Input as Conveyor != this) return;
            {
                if (conveyor.output.Input as Conveyor != this) return;
                
                if (conveyor.inventory[0].ticks >= conveyor.distanceBetweenIO * (TicksPerMinute / (float) conveyor.itemsPerMinute))
                {
                    conveyor.inventory[0].ticks = 0;
                    conveyor.output.Deposit(Withdraw());
                }
            }
        }
        
        //> DELETE CONVEYOR AND CLEAN UP
        override public void OnDestroy()
        {
            conveyor.input.DisconnectOutput(this);
            conveyor.output.DisconnectInput(this);
            conveyor.inventory.ForEach(Destroy);
            base.OnDestroy();
        }
    }
}
