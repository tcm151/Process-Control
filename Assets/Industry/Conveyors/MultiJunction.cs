using System;
using System.Collections.Generic;
using ProcessControl.Graphs;
using ProcessControl.Industry.Machines;
using ProcessControl.Industry.Resources;
using ProcessControl.Tools;
using UnityEngine;


namespace ProcessControl.Industry.Conveyors
{
    public class MultiJunction : Junction
    {
        // [Serializable] public class Data
        // {
            public bool enabled;
            public bool sleeping;
            public int ticks;
            public int sleepThreshold = 256;
            
            [Header("Input")]
            public bool inputEnabled = true;
            public int maxInputs = 1;
            public Edge currentInput;
            public List<Edge> inputs = new List<Edge>();
            
            [Header("Output")]
            public bool outputEnabled = true;
            public int maxOutputs = 1;
            public Edge currentOutput;
            public List<Edge> outputs = new List<Edge>();

            [Header("Inventory")]
            public Container inventory;
        // }
        // [SerializeField] internal Data junction;

        override public IO Input => currentInput;
        override public IO Output => currentOutput;

        public void Build()
        {
            enabled = true;
        }
        
        //> CONNECT INPUT
        override public bool ConnectInput(IO input)
        {
            if (inputs.Contains(input as Edge)) return false;
            inputs.Add(input as Edge);
            currentInput = inputs[0];
            return true;
        }

        override public bool DisconnectInput(IO input)
        {
            if (!inputs.Contains(input as Edge)) return false;
            inputs.Remove(input as Edge);
            currentInput = (inputs.Count >= 1) ? inputs[0] : null;
            return true;
        }
        
        protected void NextInput()
        {
            if (!inputEnabled || currentInput is null || maxInputs == 1) return;
            currentInput = inputs.ItemAfter(currentInput);
        }
        
        //> CONNECT OUTPUT
        override public bool ConnectOutput(IO output)
        {
            if (outputs.Contains(output as Edge)) return false;
            outputs.Add(output as Edge);
            currentOutput = outputs[0];
            return true;
        }
        
        override public bool DisconnectOutput(IO output)
        {
            if (!outputs.Contains(output as Edge)) return false;
            outputs.Remove(output as Edge);
            currentOutput = (outputs.Count >= 1) ? outputs[0] : null;
            return true;
        }
        
        protected void NextOutput()
        {
            if (!outputEnabled || currentOutput is null || maxOutputs == 1) return;
            currentOutput = outputs.ItemAfter(currentOutput);
        }


        //> DEPOSIT RESOURCES
        override public bool CanDeposit(Item item) => inventory is null;
        override public void Deposit(Container container)
        {
            container.position = Position;
            inventory = container;
        }


        //> WITHDRAW RESOURCES
        override public bool CanWithdraw() => inventory is { };
        override public Container Withdraw()
        {
            var resource = inventory;
            inventory = null;
            return resource;
        }

        private void FixedUpdate()
        {
            // if (!enabled) return;
            
            if (++ticks % (TicksPerSecond / 16) == 0)
            {
                if (Input is {} && !CanWithdraw()) NextInput();
                if (Output is {} && inventory is {} && !CanDeposit(inventory.item)) NextOutput();
            }
        }

        override public void OnDestroy()
        {
            inputs.ForEach(Destroy);
            outputs.ForEach(Destroy);
            Destroy(inventory);
            // Destroy(gameObject);
            base.OnDestroy();
        }
    }
}