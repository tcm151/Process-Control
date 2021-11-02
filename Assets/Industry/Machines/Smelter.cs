using System;
using System.Collections.Generic;
using System.Linq;
using ProcessControl.Industry.Resources;
using UnityEngine;

namespace ProcessControl.Industry.Machines
{
    public class Smelter : Machine
    {
        [Header("Smelter")]
        public int energy;
        public int maxEnergy;
        public List<Resource> acceptedFuels;
        [Range(1, 64)] public float smeltingSpeed;
        
        override protected void FixedUpdate()
        {
            base.FixedUpdate();
            
            if (++ticks % (TicksPerMinute / smeltingSpeed) == 0)
            {
                ticks = 0;
                if (inputInventory.Count == 0) return;

                if (acceptedFuels.Any(f => inputInventory.Contains(f)))
                {
                    var match = acceptedFuels.FirstOrDefault(f => inputInventory.Contains(f));
                    if (match is null)
                    {
                        Debug.Log("NO FUEL!");
                        return;
                    }

                    if (energy > maxEnergy - match.energy)
                    {
                        Debug.Log("Already enough energy...");
                        return;
                    }
                    
                    var fuel = inputInventory.Withdraw(match);
                    Debug.Log("Withdrawing fuel..");
                    
                    energy += match.energy;
                    Debug.Log($"Added {match.energy} energy...");
                }
                
                if (energy < currentRecipe.energyCost) return;
                Smelt();
            }
        }

        private void Smelt()
        {
            if (currentRecipe.inputItems.TrueForAll(recipeItem => inputInventory.Contains(recipeItem.item, recipeItem.amount)))
            {
                currentRecipe.inputItems.ForEach(i => inputInventory.Withdraw(i.item, i.amount));
                currentRecipe.outputItems.ForEach(r => outputInventory.Deposit(r.item, r.amount));
                energy -= currentRecipe.energyCost;
            }
        }
    }
}