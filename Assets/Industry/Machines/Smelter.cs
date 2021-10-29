using System;
using UnityEngine;

namespace ProcessControl.Industry.Machines
{
    public class Smelter : Machine
    {
        // [Serializable] new public class Data
        // {
            public int energy;
            public int maxEnergy;
        // }
        // [SerializeField] internal Data smelter;
        
        [Range(1, 64)] public float smeltingSpeed;

        override protected void FixedUpdate()
        {
            base.FixedUpdate();
            
            if (++ticks % (TicksPerMinute / smeltingSpeed) == 0)
            {
                ticks = 0;
                if (inputInventory.Count == 0) return;

                if (energy < maxEnergy)
                {
                    
                }
                
                Smelt();
            }
        }

        private void Smelt()
        {
            if (currentRecipe.inputItems.TrueForAll(recipeItem => inputInventory.Contains(recipeItem.item, recipeItem.amount)))
            {
                currentRecipe.inputItems.ForEach(i => inputInventory.Withdraw(i.item, i.amount));
                currentRecipe.outputItems.ForEach(r => outputInventory.Deposit(r.item, r.amount));
            }
        }
    }
}