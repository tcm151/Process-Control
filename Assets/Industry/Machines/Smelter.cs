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
                    machine.outputInventory.Add(ingot);
                }

                // var newResource = Smelt(machine.inputInventory.TakeFirst());
                // Deposit(newResource);
            }
        }

        private Entity Smelt(Entity entity)
        {
            var instance = ResourceFactory.SpawnResource(entity.resourceProperties.material, ResourceProperties.Form.Ingot, Position);
            if (instance is null) Debug.Log("NO PREFAB!");
            instance.name = $"{instance.resourceProperties.material} {instance.resourceProperties.form}.{++Entity.Count:D4}";
            
            Destroy(entity);
            
            return instance;
        }
    }
}