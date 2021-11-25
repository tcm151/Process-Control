using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ProcessControl.Jobs;
using ProcessControl.Tools;
using ProcessControl.Graphs;
#pragma warning disable 108,114


namespace ProcessControl.Industry
{
    [SelectionBase]
    public class Machine : Node , IBuildable, IInventory
    {
        public int ticks;
        public bool sleeping;
        public int sleepThreshold = 256;

        [Header("Recipes")]
        public Recipe currentRecipe;
        public List<Recipe> recipes;

        [Header("Inventory")]
        public int inventorySize = 16;
        public Inventory inputInventory;
        public Inventory outputInventory;
        
        [Header("Input")]
        public bool inputEnabled = true;
        public int maxInputs = 1;
        // public Conveyor currentInput;
        private IO currentInput;
        override public IO Input => currentInput;
        // public List<Conveyor> inputs = new List<Conveyor>();
        public readonly List<IO> inputs = new List<IO>();

        [Header("Output")]
        public bool outputEnabled = true;
        public int maxOutputs = 1;
        // public Conveyor currentOutput;
        private IO currentOutput;
        override public IO Output => currentOutput;
        // public List<Conveyor> outputs = new List<Conveyor>();
        public readonly List<IO> outputs = new List<IO>();

        
        public Task DeliverItems(List<ItemAmount> itemAmounts)
        {
            return Task.CompletedTask;
        }
        
        public async Task Build(float buildTime)
        {
            var time = 0f;
            while ((time += Time.deltaTime) < buildTime) await Task.Yield();
            renderer.color = enabledColor;
            enabled = true;
        }
        
        public async Task Disassemble(float deconstructionTime)
        {
            var time = 0f;
            while ((time += Time.deltaTime) < deconstructionTime) await Task.Yield();
            Destroy(this);
        }

        override protected void Awake()
        {
            base.Awake();
            
            inputInventory = new Inventory(maxInputs, inventorySize);
            outputInventory = new Inventory(maxOutputs, inventorySize);
            if (recipes.Count >= 1) currentRecipe = recipes[0];
        }

        
        //> CONNECT INPUT
        override public bool ConnectInput(IO input)
        {
            if (inputs.Count >= maxInputs) return false;
            if (inputs.Contains(input)) return false;
            inputs.Add(input);
            currentInput = inputs[0];
            return true;
        }
        
        //> DISCONNECT INPUT
        override public bool DisconnectInput(IO input)
        {
            if (!inputs.Contains(input)) return false;
            inputs.Remove(input);
            currentInput = (inputs.Count >= 1) ? inputs[0] : null;
            return true;
        }

        //> SWITCH TO NEXT VALID INPUT
        protected void NextInput()
        {
            if (!inputEnabled || currentInput is null || maxInputs == 1) return;
            currentInput = inputs.ItemAfter(currentInput);
        }
        
        //> CONNECT OUTPUT
        override public bool ConnectOutput(IO output)
        {
            if (outputs.Count >= maxOutputs) return false;
            if (outputs.Contains(output)) return false;
            outputs.Add(output);
            currentOutput = outputs[0];
            return true;
        }
        
        //> DISCONNECT OUTPUT
        override public bool DisconnectOutput(IO output)
        {
            if (!outputs.Contains(output)) return false;
            outputs.Remove(output);
            currentOutput = (outputs.Count >= 1) ? outputs[0] : null;
            return true;
        }

        //> SWITCH TO NEXT VALID OUTPUT
        protected void NextOutput()
        {
            if (!outputEnabled || currentOutput is null || maxOutputs == 1) return;
            currentOutput = outputs.ItemAfter(currentOutput);
        }


        //> DEPOSIT RESOURCES
        override public bool CanDeposit(Item item) => inputInventory.CanDeposit(item);
        override public void Deposit(Container container)
        {
            container.position = this.position;
            container.SetVisible(false);
            inputInventory.Deposit(container.item);
            ItemFactory.DisposeContainer(container);
            NextInput();
        }


        //> WITHDRAW RESOURCES
        override public bool CanWithdraw() => outputInventory.Count >= 1;
        override public Container Withdraw()
        {
            var item = outputInventory.Withdraw();
            var container = ItemFactory.SpawnContainer(item, position);
            container.SetVisible(true);
            NextOutput();
            return container;
        }

        //> SLEEP IF IDLE
        virtual protected void FixedUpdate()
        {
            //! these may not work properly
            if (!enabled || sleeping) return;
            if (ticks >= sleepThreshold)
            {
                sleeping = true;
                return;
            }
        }
        
        //> DESTROY AND CLEANUP MACHINE
        override public void OnDestroy()
        {
            // inputs.ForEach(Destroy);
            // outputs.ForEach(Destroy);
            inputs.ForEach(i => { if (i is Entity e) Destroy(e);});
            outputs.ForEach(o => { if (o is Entity e) Destroy(e);});
            inputInventory.Clear();
            outputInventory.Clear();
            base.OnDestroy();
        }

        public bool Contains(ItemAmount itemAmount) => inputInventory.Contains(itemAmount) || outputInventory.Contains(itemAmount);
        public ItemAmount Withdraw(ItemAmount itemAmount) => outputInventory.Withdraw(itemAmount);
        public void Deposit(ItemAmount itemAmount) => inputInventory.Deposit(itemAmount);
    }
}
