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
        // [Serializable] public class Data
        // {
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
        // }
        // [SerializeField] internal Data conveyor;

        private SpriteRenderer renderer;
        public void SetLength(float size) => renderer.size = new Vector2(size, 1);

        override public float Length { get
        {
            if (input is null || output is null) return 0;
            return Node.DistanceBetween(input, output);
        }}

        //> ADD AND REMOVE RESOURCES
        override public void Deposit(Container container) => inventory.Add(container);
        override public bool CanDeposit(Item item)
            => inventory.Count == 0
            || inventory.Count < inventorySize
            && inventory.Count >= 1
            && inventory.Last().ticks >= 2 * TicksPerSecond / itemsPerMinute;
        
        override public Container Withdraw() => inventory.TakeFirst();
        override public bool CanWithdraw()
            => inventory.Count >= 1
             && inventory[0].ticks > distanceBetweenIO * TicksPerSecond / itemsPerMinute;
        
        override public IO Input => input;
        override public IO Output => output;

        public void Build()
        {
            enabled = true;
        }

        //> CONNECT INPUT
        override public bool ConnectInput(IO input)
        {
            if (input == input as Node) return false;
            input = input as Node;
            ManageConnection();
            return true;
        }
        override public bool DisconnectInput(IO input)
        {
            if (input != input as Node) return false;
            input = null;
            ManageConnection();
            return true;
        }

        //> CONNECT OUTPUT
        override public bool ConnectOutput(IO output)
        {
            if (output == output as Node) return false;
            output = output as Node;
            ManageConnection();
            return true;
        }
        override public bool DisconnectOutput(IO output)
        {
            if (output != output as Node) return false;
            output = null;
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
            if (input is null || output is null) distanceBetweenIO = 0;
            else distanceBetweenIO = Node.DistanceBetween(input, output);
            
            SetLength(distanceBetweenIO);

            if (input is { } && output is { })
            {
                var direction = input.DirectionTo(output);
                var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            }
            
            inventorySize = Mathf.CeilToInt(distanceBetweenIO) * 2;
        }

        //> FIXED CALCULATION INTERVAL
        protected void FixedUpdate()
        {
            // if (!enabled) return;
            
            //> EVERY INTERVAL PULL FROM INPUT IF CAPABLE
            if (++ticks > (TicksPerMinute / (itemsPerMinute * 2)))
            {
                // if (inventory.Count <= 0) return;
                if (CanDeposit(null) && input.CanWithdraw() && input.Output as Conveyor == this)
                {
                    ticks = 0;
                    var resource = input.Withdraw();
                    if (resource is { }) Deposit(resource);
                }
            }

            //> MOVE RESOURCES ALONG CONVEYOR PATH
            for (int i = 0; i < inventory.Count; i++)
            {
                var resource = inventory[i];
                
                var indexPercentage = distanceBetweenIO * ((inventorySize - i) / (float) inventorySize);
                var indexPosition = input.Position + input.DirectionTo(output) * indexPercentage;

                if (resource.position != indexPosition) resource.ticks++;
                
                resource.position = Vector3.MoveTowards(resource.position, indexPosition, itemsPerMinute / (float) (TicksPerMinute));
            }


            //> DEPOSIT FIRST ITEM IN OUTPUT
            if (!CanWithdraw() || !output.CanDeposit(inventory[0].item)) return;
            {
                if (output is Junction && output.Input as Conveyor != this) return;
                
                if (inventory[0].ticks >= distanceBetweenIO * (TicksPerMinute / (float) itemsPerMinute))
                {
                    inventory[0].ticks = 0;
                    output.Deposit(Withdraw());
                }
            }
        }
        
        //> DELETE CONVEYOR AND CLEAN UP
        override public void OnDestroy()
        {
            input.DisconnectOutput(this);
            output.DisconnectInput(this);
            inventory.ForEach(Destroy);
            base.OnDestroy();
        }
    }
}
