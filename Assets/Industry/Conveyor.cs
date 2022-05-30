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
    public class Conveyor : Edge, IO, Buildable
    {
        public Recipe recipe => schematic.recipe;

        [Header("Conveyor Settings")] public int itemsPerMinute = 8;

        public IO Input {get; set;}
        public IO Output {get; set;}
        private Inventory inventory;
        public Inventory InputInventory => inventory;
        public Inventory OutputInventory => inventory;
        private int InventorySize => Mathf.CeilToInt(Length) * 2;
        public List<Container> containers = new List<Container>();
        public readonly List<Cell> tilesCovered = new List<Cell>();

        //> LENGTH ON THE CONVEYOR BASED ON CONNECTIONS
        public override float Length =>
            (Input is null || Output is null) ?
                0 : Vector3.Distance(Input.position, Output.position);

        //> CENTER OF CONVEYOR BASED ON CONNECTIONS
        public override Vector3 Center =>
            (Input is null || Output is null) ?
                default : (Input.position + Output.position) / 2f;

        //> DEPOSIT RESOURCES
        public bool CanDeposit(Item item)
            => containers.Count == 0 || containers.Count < InventorySize && containers.Count >= 1 && containers.Last().ticks >= 2 * TicksPerSecond / itemsPerMinute;
        public void Deposit(Container container)
        {
            containers.Add(container);
            inventory.Deposit(container.stack);
        }

        //> WITHDRAW RESOURCES
        public bool CanWithdraw()
            => containers.Count >= 1 && containers[0].ticks > Length * TicksPerSecond / itemsPerMinute;
        public Container Withdraw()
        {
            var firstContainer = containers.TakeAndRemoveFirst();
            inventory.Withdraw(firstContainer.stack);
            return firstContainer;
        }

        public async Task Deliver(Stack itemAmounts, float deliveryTime)
        {
            await Alerp.Delay(deliveryTime);
        }

        public async Task Build(float buildTime)
        {
            await Alerp.Delay(buildTime);
            var enabledColor = renderer.color;
            enabledColor.a = EnabledAlpha / 255f;
            renderer.color = enabledColor;
            enabled = true;
        }

        public async Task Disassemble(float deconstructionTime)
        {
            await Alerp.Delay(deconstructionTime);
            Destroy(this);
        }

        //> CONNECT INPUT
        public bool ConnectInput(IO newInput)
        {
            if (Input == newInput) return false;
            Input = newInput;
            UpdateConnections();
            return true;
        }

        public bool DisconnectInput(IO oldInput)
        {
            if (Input != oldInput) return false;
            Input = null;
            UpdateConnections();
            return true;
        }

        //> CONNECT OUTPUT
        public bool ConnectOutput(IO newOutput)
        {
            if (Output == newOutput) return false;
            Output = newOutput;
            UpdateConnections();
            return true;
        }

        public bool DisconnectOutput(IO oldOutput)
        {
            if (Output != oldOutput) return false;
            Output = null;
            UpdateConnections();
            return true;
        }

        //> INITIALIZATION
        protected override void Awake()
        {
            base.Awake();
            inventory = new Inventory(InventorySize, 1, this.transform);
        }

        //> MODIFY PROPERTIES ON CONNECTION
        private void UpdateConnections()
        {
            renderer.size = new Vector2(Length, 1);
            if (Input is null || Output is null) return;
            var direction = Input.position.DirectionTo(Output.position);
            var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }

        //> FIXED CALCULATION INTERVAL
        protected void FixedUpdate()
        {
            if (!enabled) return;

            //> EVERY INTERVAL PULL FROM INPUT IF CAPABLE
            if (++ticks > (TicksPerMinute / (itemsPerMinute * 2)))
            {
                // if (containers.Count <= 0) return;
                if (CanDeposit(null) && Input.CanWithdraw() && Input.Output as Conveyor == this)
                {
                    ticks = 0;
                    var resource = Input.Withdraw();
                    if (resource is { }) Deposit(resource);
                }
            }

            //> MOVE RESOURCES ALONG CONVEYOR PATH
            for (int i = 0; i < containers.Count; i++)
            {
                var resource = containers[i];
                var indexPercentage = Length * ((InventorySize - i) / (float)InventorySize);
                var indexPosition = Input.position + Input.position.DirectionTo(Output.position) * indexPercentage;
                if (resource.position != indexPosition) resource.ticks++;
                resource.position = Vector3.MoveTowards(resource.position, indexPosition, itemsPerMinute / (float)(TicksPerMinute));
            }


            //> DEPOSIT FIRST ITEM IN OUTPUT
            if (!CanWithdraw() || !Output.CanDeposit(containers[0].stack.item)) return;
            {
                if (Output is Junction && Output.Input as Conveyor != this) return;
                if (containers[0].ticks >= Length * (TicksPerMinute / (float)itemsPerMinute))
                {
                    containers[0].ticks = 0;
                    Output.Deposit(Withdraw());
                }
            }
        }

        //> DELETE CONVEYOR AND CLEAN UP
        protected override void OnDestroy()
        {
            Input.DisconnectOutput(this);
            Output.DisconnectInput(this);
            tilesCovered.ForEach(t => t.edges.Remove(this));
            containers.ForEach(Destroy);
            base.OnDestroy();
        }
    }
}