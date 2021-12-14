using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using ProcessControl.Jobs;
using ProcessControl.Graphs;
using ProcessControl.Tools;


namespace ProcessControl.Industry
{
    public class Junction : Node, IO, Buildable
    {
        public Recipe recipe => schematic.recipe;

        [Header("IO")]
        //- input
        public bool inputEnabled = true;
        public int maxInputs = 1;
        private IO currentInput;
        private readonly List<IO> inputs = new List<IO>();
        //- output
        public bool outputEnabled = true;
        public int maxOutputs = 1;
        private IO currentOutput;
        private readonly List<IO> outputs = new List<IO>();
        //- inventory
        public Container inventory;

        // public event Action onAllItemsDelivered;
        
        public Task Deliver(Stack itemAmounts)
        {
            return Task.CompletedTask;
            // if (schematic.recipe.inputItems.TrueForAll(itemAmounts.Contains)) return Task.CompletedTask;
            
            // Debug.Log($"Failed delivery to {schematic.name}", this);
            // return Task.FromException(new Exception($"Failed delivery to {schematic.name}"));
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
        
        
        virtual public IO Input => currentInput;
        virtual public IO Output => currentOutput;

        //> CONNECT INPUT
        virtual public bool ConnectInput(IO input)
        {
            if (inputs.Contains(input)) return false;
            inputs.Add(input);
            currentInput = inputs[0];
            return true;
        }

        virtual public bool DisconnectInput(IO input)
        {
            if (!inputs.Contains(input)) return false;
            inputs.Remove(input);
            currentInput = (inputs.Count >= 1) ? inputs[0] : null;
            return true;
        }
        
        protected void NextInput()
        {
            if (!inputEnabled || currentInput is null || maxInputs == 1) return;
            currentInput = inputs.ItemAfter(currentInput);
        }
        
        //> CONNECT OUTPUT
        virtual public bool ConnectOutput(IO output)
        {
            if (outputs.Contains(output)) return false;
            outputs.Add(output);
            currentOutput = outputs[0];
            return true;
        }
        
        virtual public bool DisconnectOutput(IO output)
        {
            if (!outputs.Contains(output)) return false;
            outputs.Remove(output);
            currentOutput = (outputs.Count >= 1) ? outputs[0] : null;
            return true;
        }
        
        protected void NextOutput()
        {
            if (!outputEnabled || currentOutput is null || maxOutputs == 1) return;
            currentOutput = outputs.ItemAfter(currentOutput);
        }


        //> DEPOSIT RESOURCES
        virtual public bool CanDeposit(Item item) => inventory is null;
        virtual public void Deposit(Container container)
        {
            container.position = position;
            inventory = container;
        }


        //> WITHDRAW RESOURCES
        virtual public bool CanWithdraw() => inventory is { };
        virtual public Container Withdraw()
        {
            var resource = inventory;
            inventory = null;
            return resource;
        }

        private void FixedUpdate()
        {
            if (++ticks % (TicksPerSecond / 16) == 0)
            {
                if (Input is {} && !CanWithdraw()) NextInput();
                if (Output is {} && inventory is {} && !CanDeposit(inventory.item)) NextOutput();
            }
        }

        override public void OnDestroy()
        {
            inputs.ForEach(i => Destroy(i as Entity));
            outputs.ForEach(o => Destroy(o as Entity));
            Destroy(inventory);
            base.OnDestroy();
        }
    }
}