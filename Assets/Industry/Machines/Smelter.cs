using ProcessControl.Industry.Resources;
using ProcessControl.Tools;
using UnityEngine;


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
                

                var resource = machine.inputInventory.TakeFirst();
                // if (resource is Ore)
                {
                    var ingot = Smelt(resource);
                    if (ingot is null) return;
                    machine.outputInventory.Add(ingot);
                }

                // var newResource = Smelt(machine.inputInventory.TakeFirst());
                // Deposit(newResource);
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