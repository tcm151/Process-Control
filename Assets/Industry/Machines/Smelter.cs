﻿using System.Linq;
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
                
                if (machine.currentRecipe.requiredItems.TrueForAll(requiredItem => machine.inputInventory.Count(e => e.item == requiredItem) > 2f))
                {
                    var resources = machine.inputInventory.TakeRange(0, 1);
                    var newResources = ItemFactory.Instance.SpawnItems(machine.currentRecipe.resultingItems, Position);
                    newResources.ForEach(r => machine.outputInventory.Add(r));
                    onInventoryModified?.Invoke();
                    
                    resources.ForEach(Destroy);

                    // var ingot = Smelt(resource);
                    // if (ingot is null) return;
                    // machine.outputInventory.Add(ingot);
                    // onInventoryModified?.Invoke();
                }
            }
        }

        private Entity Smelt(Entity entity)
        {
            if (entity.item is Resource resource)
            {
                var instance = ResourceFactory.SpawnResource(resource.material, Resource.Form.Ingot, Position);
                if (instance is null) Debug.Log("NO PREFAB!");
                instance.name = $"{resource.material} {resource.form}.{++Entity.Count:D4}";
                
                Destroy(entity);
                
                return instance;
            }

            return null;
        }
    }
}