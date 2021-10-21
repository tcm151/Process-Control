using System.Linq;
using UnityEngine;
using ProcessControl.Tools;
using ProcessControl.Industry.Resources;


namespace ProcessControl.Industry.Machines
{
    public class Smelter : Machine
    {
        [Range(1, 64)] public float smeltingSpeed;

        override protected void FixedUpdate()
        {
            base.FixedUpdate();
            
            if (++machine.ticks % (TicksPerMinute / smeltingSpeed) == 0)
            {
                machine.ticks = 0;
                if (machine.inputInventory.Count == 0) return;
                
                Smelt();
            }
        }

        private void Smelt()
        {
            if (machine.currentRecipe.requiredItems.TrueForAll(requiredItem => machine.inputInventory.Has(requiredItem, 1)))
            {
                var inputItems = machine.inputInventory.Withdraw(2);
                // var newResources = ItemFactory.Instance.SpawnItems(machine.currentRecipe.resultingItems, Position);
                machine.currentRecipe.resultingItems.ForEach(r => machine.outputInventory.Deposit(r));
                    
                // inputItems.ForEach(Destroy);

                // var ingot = Smelt(resource);
                // if (ingot is null) return;
                // machine.outputInventory.Add(ingot);
                // onInventoryModified?.Invoke();
            }
        }
    }
}