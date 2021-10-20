using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using ProcessControl.Tools;
using ProcessControl.Graphs;
using ProcessControl.Industry.Conveyors;
using ProcessControl.Industry.Resources;


namespace ProcessControl.Industry.Machines
{
    [SelectionBase]
    public class Machine : Node
    {
        //> MACHINE DATA CONTAINER
        [Serializable] public class Data
        {
            public bool enabled;
            public bool sleeping;
            public int ticks;
            public int sleepThreshold = 256;

            [Header("Recipes")]
            public Recipe currentRecipe;
            public List<Recipe> recipes;

            [Header("Inventory")]
            public int inventorySize = 8;
            
            [Header("Input")]
            public bool inputEnabled = true;
            public int maxInputs = 1;
            public Conveyor currentInput;
            public List<Conveyor> inputs = new List<Conveyor>();
            public Inventory inputInventoryTest;

            [Header("IOutput")]
            public bool outputEnabled = true;
            public int maxOutputs = 1;
            public Conveyor currentOutput;
            public List<Conveyor> outputs = new List<Conveyor>();
            public Inventory outputInventoryTest;
        }
        [SerializeField] internal Data machine;


        public async void Build()
        {
            machine.enabled = true;
            Debug.Log("Starting to build machine...");
            await Task.Delay(2500);
            Debug.Log("Machine built.");
        }

        private void Awake()
        {
            machine.inputInventoryTest = new Inventory(machine.maxInputs, machine.inventorySize);
            machine.outputInventoryTest = new Inventory(machine.maxOutputs, machine.inventorySize);
            
            if (machine.recipes.Count >= 1)
            {
                machine.currentRecipe = machine.recipes[0];
            }
        }

        //> IO INTERFACE
        override public IO Input => machine.currentInput;
        override public IO Output => machine.currentOutput;
        
        //> CONNECT INPUT
        override public bool ConnectInput(IO input)
        {
            if (machine.inputs.Contains(input as Conveyor)) return false;
            machine.inputs.Add(input as Conveyor);
            machine.currentInput = machine.inputs[0];
            return true;
        }
        
        //> DISCONNECT INPUT
        override public bool DisconnectInput(IO input)
        {
            if (!machine.inputs.Contains(input as Conveyor)) return false;
            machine.inputs.Remove(input as Conveyor);
            machine.currentInput = (machine.inputs.Count >= 1) ? machine.inputs[0] : null;
            return true;
        }

        //> SWITCH TO NEXT VALID INPUT
        protected void NextInput()
        {
            if (!machine.inputEnabled || machine.currentInput is null || machine.maxInputs == 1) return;
            machine.currentInput = machine.inputs.ItemAfter(machine.currentInput);
        }
        
        //> CONNECT OUTPUT
        override public bool ConnectOutput(IO output)
        {
            if (machine.outputs.Contains(output as Conveyor)) return false;
            machine.outputs.Add(output as Conveyor);
            machine.currentOutput = machine.outputs[0];
            return true;
        }
        
        //> DISCONNECT OUTPUT
        override public bool DisconnectOutput(IO output)
        {
            if (!machine.outputs.Contains(output as Conveyor)) return false;
            machine.outputs.Remove(output as Conveyor);
            machine.currentOutput = (machine.outputs.Count >= 1) ? machine.outputs[0] : null;
            return true;
        }

        //> SWITCH TO NEXT VALID OUTPUT
        protected void NextOutput()
        {
            if (!machine.outputEnabled || machine.currentOutput is null || machine.maxOutputs == 1) return;
            machine.currentOutput = machine.outputs.ItemAfter(machine.currentOutput);
        }


        //> DEPOSIT RESOURCES
        // override public bool CanDeposit => machine.inputInventory.Count < machine.inventorySize;
        override public bool CanDeposit => machine.inputInventoryTest.Count < machine.inventorySize;
        override public void Deposit(Container container)
        {
            container.position = this.Position;
            container.SetVisible(false);
            // machine.inputInventoryTest.Deposit(resource);
            // machine.inputInventory.Add(container);
            machine.inputInventoryTest.Deposit(container);
            // onInventoryModified?.Invoke();
            NextInput();
        }


        //> WITHDRAW RESOURCES
        // override public bool CanWithdraw => machine.outputInventory.Count >= 1;
        override public bool CanWithdraw => machine.outputInventoryTest.Count >= 1;
        override public Container Withdraw()
        {
            // var resource = machine.outputInventory.TakeFirst();
            var resource = machine.outputInventoryTest.Withdraw();
            // onInventoryModified?.Invoke();
            resource.SetVisible(true);
            NextOutput();
            return resource;
        }

        //> SLEEP IF IDLE
        virtual protected void FixedUpdate()
        {
            if (!machine.enabled || machine.sleeping) return;
            if (machine.ticks >= machine.sleepThreshold)
            {
                machine.sleeping = true;
                return;
            }
        }
        
        //> DESTROY AND CLEANUP MACHINE
        override public void OnDestroy()
        {
            machine.inputs.ForEach(Destroy);
            machine.outputs.ForEach(Destroy);
            // machine.inputInventory.ForEach(Destroy);
            // machine.outputInventory.ForEach(Destroy);
            machine.inputInventoryTest.Clear();
            machine.outputInventoryTest.Clear();
            // Destroy(gameObject);
            base.OnDestroy();
        }
    }
}
