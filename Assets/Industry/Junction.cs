using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ProcessControl.Jobs;
using ProcessControl.Graphs;
using ProcessControl.Industry.Resources;
using UnityEngine;


namespace ProcessControl.Industry.Conveyors
{
    abstract public class Junction : Node, IBuildable
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
        public Container inventory;
        
        abstract override public IO Input {get;}
        abstract override public IO Output {get;}
        
        abstract override public bool ConnectInput(IO input);
        abstract override public bool DisconnectInput(IO input);
        abstract override public bool ConnectOutput(IO output);
        abstract override public bool DisconnectOutput(IO output);

        abstract override public bool CanDeposit(Item item);
        abstract override public void Deposit(Container container);

        abstract override public bool CanWithdraw();
        abstract override public Container Withdraw();

        override public void OnDestroy()
        {
            inputs.ForEach(Destroy);
            outputs.ForEach(Destroy);
            Destroy(inventory);
            base.OnDestroy();
        }
        
        public async Task Build(int buildTime)
        {
            var time = 0f;
            while ((time += Time.deltaTime) < buildTime) await Task.Yield();
            // renderer.color = enabledColor;
            enabled = true;
        }
        
        public async Task Deconstruct(int deconstructionTime)
        {
            var time = 0f;
            while ((time += Time.deltaTime) < deconstructionTime) await Task.Yield();
            Destroy(this);
        }
    }
}