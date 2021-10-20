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
                
                if (machine.currentRecipe.requiredItems.TrueForAll(requiredItem => machine.inputInventory.Has(requiredItem, 2)))
                {
                    var resources = machine.inputInventory.Withdraw(2);
                    var newResources = ItemFactory.Instance.SpawnItems(machine.currentRecipe.resultingItems, Position);
                    newResources.ForEach(r => machine.outputInventory.Deposit(r));
                    // onInventoryModified?.Invoke();
                    
                    resources.ForEach(Destroy);

                    // var ingot = Smelt(resource);
                    // if (ingot is null) return;
                    // machine.outputInventory.Add(ingot);
                    // onInventoryModified?.Invoke();
                }
            }
        }

        private Container Smelt(Container container)
        {
            if (container.item is Resource resource)
            {
                var instance = ResourceFactory.SpawnResource(resource.material, Resource.Form.Ingot, Position);
                if (instance is null) Debug.Log("NO PREFAB!");
                instance.name = $"{resource.material} {resource.form}.{++Container.Count:D4}";
                
                Destroy(container);
                
                return instance;
            }

            return null;
        }
    }
}