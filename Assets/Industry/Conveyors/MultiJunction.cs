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
        [Serializable] public class Data
        {
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
            public Resource inventory;
        }
        [SerializeField] internal Data junction;

        override public IO Input => junction.currentInput;
        override public IO Output => junction.currentOutput;
        
        //> CONNECT INPUT
        override public bool ConnectInput(IO input)
        {
            if (junction.inputs.Contains(input as Edge)) return false;
            junction.inputs.Add(input as Edge);
            junction.currentInput = junction.inputs[0];
            return true;
        }

        override public bool DisconnectInput(IO input)
        {
            if (!junction.inputs.Contains(input as Edge)) return false;
            junction.inputs.Remove(input as Edge);
            junction.currentInput = (junction.inputs.Count >= 1) ? junction.inputs[0] : null;
            return true;
        }
        
        protected void NextInput()
        {
            if (!junction.inputEnabled || junction.currentInput is null || junction.maxInputs == 1) return;
            junction.currentInput = junction.inputs.ItemAfter(junction.currentInput);
        }
        
        //> CONNECT OUTPUT
        override public bool ConnectOutput(IO output)
        {
            if (junction.outputs.Contains(output as Edge)) return false;
            junction.outputs.Add(output as Edge);
            junction.currentOutput = junction.outputs[0];
            return true;
        }
        
        override public bool DisconnectOutput(IO output)
        {
            if (!junction.outputs.Contains(output as Edge)) return false;
            junction.outputs.Remove(output as Edge);
            junction.currentOutput = (junction.outputs.Count >= 1) ? junction.outputs[0] : null;
            return true;
        }
        
        protected void NextOutput()
        {
            if (!junction.outputEnabled || junction.currentOutput is null || junction.maxOutputs == 1) return;
            junction.currentOutput = junction.outputs.ItemAfter(junction.currentOutput);
        }


        //> DEPOSIT RESOURCES
        override public bool CanDeposit => junction.inventory is null;
        override public void Deposit(Resource resource)
        {
            resource.position = Position;
            junction.inventory = resource;
        }


        //> WITHDRAW RESOURCES
        override public bool CanWithdraw => junction.inventory is { };
        override public Resource Withdraw()
        {
            var resource = junction.inventory;
            junction.inventory = null;
            return resource;
        }

        private void FixedUpdate()
        {
            //@ add sleeping when idle
            
            if (++junction.ticks % (TicksPerSecond / 16) == 0)
            {
                if (Input is {CanWithdraw: false}) NextInput();
                if (Output is {CanDeposit: false}) NextOutput();
            }
        }

        override public void OnDestroy()
        {
            junction.inputs.ForEach(Destroy);
            junction.outputs.ForEach(Destroy);
            Destroy(junction.inventory);
            Destroy(gameObject);
        }
    }
}