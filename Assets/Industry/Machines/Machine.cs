using System;
using System.Collections.Generic;
using UnityEngine;
using ProcessControl.Tools;
using ProcessControl.Graphs;
using ProcessControl.Procedural;
using ProcessControl.Industry.Resources;


namespace ProcessControl.Industry.Machines
{
    [SelectionBase]
    public class Machine : Node
    {
        //> MACHINE DATA CONTAINER
        [Serializable] public class Data
        {
            public ProceduralGrid.Cell cell;
            
            public bool sleeping;
            public int ticks;
            public int sleepThreshold = 256;

            [Header("Inventory")]
            public int inventorySize = 8;
            // public List<Resource> inventory = new List<Resource>();
            
            [Header("Input")]
            public bool inputEnabled = true;
            public int maxInputs = 1;
            public Edge currentInput;
            public List<Edge> inputs = new List<Edge>();
            public List<Resource> inputInventory = new List<Resource>();
            
            [Header("IOutput")]
            public bool outputEnabled = true;
            public int maxOutputs = 1;
            public Edge currentOutput;
            public List<Edge> outputs = new List<Edge>();
            public List<Resource> outputInventory = new List<Resource>();
        }
        [SerializeField] internal Data machine;

        //> IO INTERFACE
        override public IO Input => machine.currentInput;
        override public IO Output => machine.currentOutput;
        
        //> PROPERTIES
        // virtual public bool Full => node.inputInventory.Count >= InventorySize;
        // virtual public bool Empty => node.outputInventory.Count == 0;
        // virtual public int InventorySize => node.inventorySize;

        //> CONNECT INPUT
        override public bool ConnectInput(IO input)
        {
            if (machine.inputs.Contains(input as Edge)) return false;
            machine.inputs.Add(input as Edge);
            machine.currentInput = machine.inputs[0];
            return true;
        }
        
        override public bool DisconnectInput(IO input)
        {
            if (!machine.inputs.Contains(input as Edge)) return false;
            machine.inputs.Remove(input as Edge);
            machine.currentInput = (machine.inputs.Count >= 1) ? machine.inputs[0] : null;
            return true;
        }

        protected void NextInput()
        {
            if (!machine.inputEnabled || machine.currentInput is null || machine.maxInputs == 1) return;
            machine.currentInput = machine.inputs.ItemAfter(machine.currentInput);
        }
        
        //> CONNECT OUTPUT
        override public bool ConnectOutput(IO output)
        {
            if (machine.outputs.Contains(output as Edge)) return false;
            machine.outputs.Add(output as Edge);
            machine.currentOutput = machine.outputs[0];
            return true;
        }
        
        override public bool DisconnectOutput(IO output)
        {
            if (!machine.outputs.Contains(output as Edge)) return false;
            machine.outputs.Remove(output as Edge);
            machine.currentOutput = (machine.outputs.Count >= 1) ? machine.outputs[0] : null;
            return true;
        }

        protected void NextOutput()
        {
            if (!machine.outputEnabled || machine.currentOutput is null || machine.maxOutputs == 1) return;
            machine.currentOutput = machine.outputs.ItemAfter(machine.currentOutput);
        }


        //> DEPOSIT RESOURCES
        override public bool CanDeposit => machine.inputInventory.Count < machine.inventorySize;
        override public void Deposit(Resource resource)
        {
            resource.data.position = Position;
            resource.SetVisible(false);
            machine.inputInventory.Add(resource);
            NextInput();
        }


        //> WITHDRAW RESOURCES
        override public bool CanWithdraw => machine.outputInventory.Count >= 1;
        override public Resource Withdraw()
        {
            var resource = machine.outputInventory.TakeFirst();
            resource.SetVisible(true);
            NextOutput();
            return resource;
        }

        //> FIXED CALCULATION INTERVAL
        virtual protected void FixedUpdate()
        {
            if (machine.sleeping) return;
            if (machine.ticks >= machine.sleepThreshold) machine.sleeping = true;
        }
        
        //> DESTROY AND CLEANUP MACHINE
        override public void OnDestroy()
        {
            machine.inputs.ForEach(Destroy);
            machine.outputs.ForEach(Destroy);
            machine.inputInventory.ForEach(Destroy);
            machine.outputInventory.ForEach(Destroy);
            Destroy(gameObject);
        }
    }
}
