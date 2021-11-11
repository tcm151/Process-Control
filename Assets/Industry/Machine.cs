using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using ProcessControl.Jobs;
using ProcessControl.Tools;
using ProcessControl.Graphs;
#pragma warning disable 108,114


namespace ProcessControl.Industry
{
    [SelectionBase]
    public class Machine : Node , IBuildable
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
        public Conveyor currentInput;
        public List<Conveyor> inputs = new List<Conveyor>();

        [Header("Output")]
        public bool outputEnabled = true;
        public int maxOutputs = 1;
        public Conveyor currentOutput;
        public List<Conveyor> outputs = new List<Conveyor>();

        // public Color enabledColor = new Color(255, 255, 255, 255);
        // public Color disabledColor = new Color(255, 255, 255, 100);
        
        // private SpriteRenderer renderer;


        public async Task Build(int buildTime)
        {
            var time = 0f;
            while ((time += Time.deltaTime) < buildTime) await Task.Yield();
            renderer.color = enabledColor;
            enabled = true;
        }
        
        public async Task Deconstruct(int deconstructionTime)
        {
            var time = 0f;
            while ((time += Time.deltaTime) < deconstructionTime) await Task.Yield();
            Destroy(this);
        }

        override protected void Awake()
        {
            base.Awake();
            // renderer = GetComponent<SpriteRenderer>();
            // renderer.color = disabledColor;
            
            inputInventory = new Inventory(maxInputs, inventorySize);
            outputInventory = new Inventory(maxOutputs, inventorySize);
            if (recipes.Count >= 1) currentRecipe = recipes[0];
        }

        //> IO INTERFACE
        override public IO Input => currentInput;
        override public IO Output => currentOutput;
        
        //> CONNECT INPUT
        override public bool ConnectInput(IO input)
        {
            if (inputs.Count >= maxInputs) return false;
            if (inputs.Contains(input as Conveyor)) return false;
            inputs.Add(input as Conveyor);
            currentInput = inputs[0];
            return true;
        }
        
        //> DISCONNECT INPUT
        override public bool DisconnectInput(IO input)
        {
            if (!inputs.Contains(input as Conveyor)) return false;
            inputs.Remove(input as Conveyor);
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
            if (outputs.Contains(output as Conveyor)) return false;
            outputs.Add(output as Conveyor);
            currentOutput = outputs[0];
            return true;
        }
        
        //> DISCONNECT OUTPUT
        override public bool DisconnectOutput(IO output)
        {
            if (!outputs.Contains(output as Conveyor)) return false;
            outputs.Remove(output as Conveyor);
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
            container.position = this.Position;
            container.SetVisible(false);
            inputInventory.Deposit(container.item);
            Destroy(container);
            NextInput();
        }


        //> WITHDRAW RESOURCES
        override public bool CanWithdraw() => outputInventory.Count >= 1;
        override public Container Withdraw()
        {
            var item = outputInventory.Withdraw();
            var container = ItemFactory.Instance.SpawnItem(item, this.Position);
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
            inputs.ForEach(Destroy);
            outputs.ForEach(Destroy);
            inputInventory.Clear();
            outputInventory.Clear();
            base.OnDestroy();
        }
    }
}
