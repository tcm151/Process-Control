using System;
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
    public class Machine : Node, IO, Buildable // HasInventory
    {
        public Recipe recipe => schematic.recipe;

        [Header("Recipes")]
        public Recipe currentRecipe;
        public List<Recipe> recipes;

        [Header("HasInventory")]
        public int inventorySize = 16;
        public Inventory inputInventory;
        public Inventory InputInventory => inputInventory;
        public Inventory outputInventory;
        public Inventory OutputInventory => outputInventory;
        
        [Header("Input")]
        public bool inputEnabled = true;
        public int maxInputs = 1;
        public IO Input {get; set;}
        public readonly List<IO> inputs = new List<IO>();


        [Header("Output")]
        public bool outputEnabled = true;
        public int maxOutputs = 1;
        public IO Output {get; set;}
        private readonly List<IO> outputs = new List<IO>();

        public async Task Deliver(Stack itemAmounts, float deliveryTime)
        {
            await Alerp.Delay(deliveryTime);
        }
        
        public async Task Build(float buildTime)
        {
            var time = 0f;
            while ((time += Time.deltaTime) < buildTime) await Task.Yield();
            var enabledColor = renderer.color;
            enabledColor.a = EnabledAlpha / 255f;
            renderer.color = enabledColor;
            enabled = true;
        }
        
        public async Task Disassemble(float deconstructionTime)
        {
            var time = 0f;
            while ((time += Time.deltaTime) < deconstructionTime) await Task.Yield();
            Destroy(this);
        }

        protected override void Awake()
        {
            base.Awake();
            
            inputInventory = new Inventory(maxInputs, inventorySize, this.transform);
            outputInventory = new Inventory(maxOutputs, inventorySize, this.transform);
            if (recipes.Count >= 1) currentRecipe = recipes[0];
        }

        
        //> CONNECT INPUT
        virtual public bool ConnectInput(IO input)
        {
            if (inputs.Count >= maxInputs) return false;
            if (inputs.Contains(input)) return false;
            inputs.Add(input);
            Input = inputs[0];
            return true;
        }
        
        //> DISCONNECT INPUT
        virtual public bool DisconnectInput(IO input)
        {
            if (!inputs.Contains(input)) return false;
            inputs.Remove(input);
            Input = (inputs.Count >= 1) ? inputs[0] : null;
            return true;
        }

        //> SWITCH TO NEXT VALID INPUT
        protected void NextInput()
        {
            if (!inputEnabled || Input is null || maxInputs == 1) return;
            Input = inputs.ItemAfter(Input);
        }
        
        //> CONNECT OUTPUT
        virtual public bool ConnectOutput(IO output)
        {
            if (outputs.Count >= maxOutputs) return false;
            if (outputs.Contains(output)) return false;
            outputs.Add(output);
            Output = outputs[0];
            return true;
        }
        
        //> DISCONNECT OUTPUT
        virtual public bool DisconnectOutput(IO output)
        {
            if (!outputs.Contains(output)) return false;
            outputs.Remove(output);
            Output = (outputs.Count >= 1) ? outputs[0] : null;
            return true;
        }

        //> SWITCH TO NEXT VALID OUTPUT
        protected void NextOutput()
        {
            if (!outputEnabled || Output is null || maxOutputs == 1) return;
            Output = outputs.ItemAfter(Output);
        }


        //> DEPOSIT RESOURCES
        virtual public bool CanDeposit(Item item) => inputInventory.CanDeposit(new Stack{item = item, amount = 1});
        virtual public void Deposit(Container container)
        {
            container.position = this.position;
            container.SetVisible(false);
            inputInventory.Deposit(container.stack);
            ItemFactory.DisposeContainer(container);
            NextInput();
        }


        //> WITHDRAW RESOURCES
        virtual public bool CanWithdraw() => outputInventory.Count >= 1;
        virtual public Container Withdraw()
        {
            var stack = outputInventory.Withdraw();
            var container = ItemFactory.SpawnContainer(stack.item, position);
            container.SetVisible(true);
            NextOutput();
            return container;
        }

        //> SLEEP IF IDLE
        // virtual protected void FixedUpdate()
        // {
        //     //! these may not work properly
        //     // if (!enabled || sleeping) return;
        //     // if (ticks >= sleepThreshold)
        //     // {
        //     //     sleeping = true;
        //     //     return;
        //     // }
        // }
        
        //> DESTROY AND CLEANUP MACHINE
        public override void OnDestroy()
        {
            // inputs.ForEach(Destroy);
            // outputs.ForEach(Destroy);
            inputs.ForEach(i => { if (i is Entity e) Destroy(e);});
            outputs.ForEach(o => { if (o is Entity e) Destroy(e);});
            inputInventory.Clear();
            outputInventory.Clear();
            base.OnDestroy();
        }

        public bool Contains(Stack stack) => inputInventory.Contains(stack) || outputInventory.Contains(stack);
        public Stack Withdraw(Stack stack) => outputInventory.Withdraw(stack);
        public void Deposit(Stack stack) => inputInventory.Deposit(stack);
    }
}
