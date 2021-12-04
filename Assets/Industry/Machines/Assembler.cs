using System.Threading.Tasks;
using UnityEngine;


namespace ProcessControl.Industry.Machines
{
    public class Assembler : Machine
    {
        private bool assembling;
        
        virtual protected void FixedUpdate()
        {
            if (assembling) return;

            if (currentRecipe.inputItems.TrueForAll(itemAmount => inputInventory.Contains(itemAmount)))
            {
                AssembleRecipe();
            }
        }
        
        private async void AssembleRecipe()
        {
            assembling = true;

            while (ticks++ < currentRecipe.assemblyTime)
            {
                await Task.Yield();
            }
            
            currentRecipe.outputItems.ForEach(
                i =>
                {
                    outputInventory.Deposit(i);    
                });

            ticks = 0;
            assembling = false;
        }

        public void CreateJob()
        {
            
        }
    }
    
}