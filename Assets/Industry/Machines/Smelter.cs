using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ProcessControl.Industry.Machines
{
    public class Smelter : Machine
    {
        [Header("Smelter")]
        public float energy;
        public float maxEnergy;
        public List<Resource> acceptedFuels;
        [Range(1, 64)] public float smeltingSpeed;
        
        virtual protected void FixedUpdate()
        {
            if (++ticks % (TicksPerMinute / smeltingSpeed) == 0)
            {
                ticks = 0;
                if (inputInventory.Count == 0) return;

                if (acceptedFuels.Any(f => inputInventory.Contains(new Stack{item = f})))
                {
                    var match = acceptedFuels.FirstOrDefault(f => inputInventory.Contains(new Stack{item = f}));
                    if (match is null)
                    {
                        // Debug.Log("NO FUEL!");
                        return;
                    }

                    if (energy > maxEnergy - match.energy)
                    {
                        // Debug.Log("Already enough energy...");
                        return;
                    }
                    
                    var fuel = inputInventory.Withdraw(new Stack{item = match, amount = 1});
                    // Debug.Log("Withdrawing fuel..");
                    
                    energy += match.energy;
                    // Debug.Log($"Added {match.energy} energy...");
                }
                
                if (energy < currentRecipe.energyCost) return;
                Smelt();
            }
        }

        private void Smelt()
        {
            if (currentRecipe.inputItems.TrueForAll(recipeItem => inputInventory.Contains(recipeItem)))
            {
                currentRecipe.inputItems.ForEach(i => inputInventory.Withdraw(i));
                currentRecipe.outputItems.ForEach(i => outputInventory.Deposit(i));
                energy -= currentRecipe.energyCost;
            }
        }
    }
}