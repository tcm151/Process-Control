using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using ProcessControl.Tools;
using ProcessControl.Graphs;
using ProcessControl.Jobs;
using ProcessControl.Procedural;
#pragma warning disable 108,114


namespace ProcessControl.Industry
{
    [SelectionBase]
    public class Conveyor : Edge, IBuildable
    {
        public bool enabled;
        public bool sleeping;
        public int ticks;
        public int sleepThreshold = 256;
        
        [Header("Conveyor Speed")]
        public int itemsPerMinute = 8;

        [Header("IO")]
        // public Node input;
        public IO input;
        // public Node output;
        public IO output;
        public float distanceBetweenIO;

        [Header("Inventory")]
        public int inventorySize = 8;
        public List<Container> inventory = new List<Container>();

        public readonly List<Cell> tilesCovered = new List<Cell>();


        // private SpriteRenderer renderer;
        public void SetLength(float size) => renderer.size = new Vector2(size, 1);

        public Vector3 position
        {
            get => transform.position;
            set => transform.position = value;
        }
        
        override public float Length { get
        {
            if (input is null || output is null) return 0;
            // return Node.DistanceBetween(input, output);
            return Vector3.Distance(input.position, output.position);
        }}
        
        override public Vector3 Center { get
        {
            if (input is null || output is null) return default;
            // return Node.Center(input, output);
            return (input.position + output.position) / 2f;
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

        // public event Action onAllItemsDelivered;

        public Task DeliverItems(List<ItemAmount> itemAmounts)
        {
            return Task.CompletedTask;
        }
        
        public async Task Build(float buildTime)
        {
            var time = 0f;
            while ((time += Time.deltaTime) < buildTime) await Task.Yield();
            // renderer.color = enabledColor;
            enabled = true;
        }
        
        public async Task Deconstruct(float deconstructionTime)
        {
            var time = 0f;
            while ((time += Time.deltaTime) < deconstructionTime) await Task.Yield();
            Destroy(this);
        }

        //> CONNECT INPUT
        override public bool ConnectInput(IO newInput)
        {
            if (input == newInput) return false;
            input = newInput;
            ManageConnection();
            return true;
        }
        override public bool DisconnectInput(IO oldInput)
        {
            if (input != oldInput) return false;
            input = null;
            ManageConnection();
            return true;
        }

        //> CONNECT OUTPUT
        override public bool ConnectOutput(IO newOutput)
        {
            if (output == newOutput) return false;
            output = newOutput;
            ManageConnection();
            return true;
        }
        override public bool DisconnectOutput(IO oldOutput)
        {
            if (output != oldOutput) return false;
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
            else distanceBetweenIO = Vector3.Distance(input.position, output.position);
            
            SetLength(distanceBetweenIO);

            if (input is { } && output is { })
            {
                var direction = input.position.DirectionTo(output.position);
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
                var indexPosition = input.position + input.position.DirectionTo(output.position) * indexPercentage;

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
            tilesCovered.ForEach(t => t.edges.Remove(this));
            inventory.ForEach(Destroy);
            base.OnDestroy();
        }
    }
}
