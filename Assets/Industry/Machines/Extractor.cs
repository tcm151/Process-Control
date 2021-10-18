using ProcessControl.Industry.Resources;
using UnityEngine;
using ProcessControl.Tools;


namespace ProcessControl.Industry.Machines
{
    public class Extractor : Machine
    {
        [Range(0, 64)] public float extractionSpeed;

        //> FIXED CALCULATION INTERVAL
        override protected void FixedUpdate()
        {
            if (!machine.enabled || machine.sleeping) return;
            if (machine.ticks >= machine.sleepThreshold)
            {
                machine.sleeping = true;
                return;
            }
            
            // sleep if ground is depleted
            if (parentCell.resourceDeposits.Count == 0 || parentCell.resourceDeposits[0].quantity <= 0)
            {
                machine.sleeping = true;
                return;
            }
            
            // extraction interval check
            if (++machine.ticks > (TicksPerMinute / extractionSpeed))
            {
                machine.ticks = 0;
                if (machine.outputInventory.Count >= machine.inventorySize) return;

                var resource = ExtractResource();
                onInventoryModified?.Invoke();
                machine.outputInventory.Add(resource);
            }
        }

        //> DEPOSIT RESOURCE INTO INVENTORY
        override public void Deposit(Entity entity)
        {
            entity.position = Position;
            machine.outputInventory.Add(entity);
            onInventoryModified?.Invoke();
            entity.SetVisible(false);
        }

        //> EXTRACT RESOURCE FROM THE GROUND
        private Entity ExtractResource()
        {
            parentCell.resourceDeposits[0].quantity--;
            var prefab = parentCell.resourceDeposits[0];
            var instance = ResourceFactory.SpawnResource(prefab.material, prefab.type, Position);
            if (instance is null) Debug.Log("NO PREFAB!");
            return instance;
        }
    }
}
