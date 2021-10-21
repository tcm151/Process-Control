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

                ExtractResource();
                // machine.outputInventory.Deposit(resource);
                // machine.outputInventory.Add(resource);
                // onInventoryModified?.Invoke();
            }
        }

        //> DEPOSIT RESOURCE INTO INVENTORY
        // override public void Deposit(Container container)
        // {
        //     // container.position = Position;
        //     // machine.outputInventory.Add(container);
        //     machine.outputInventory.Deposit(container.item);
        //     Destroy(container);
        //     // onInventoryModified?.Invoke();
        //     // container.SetVisible(false);
        // }

        //> EXTRACT RESOURCE FROM THE GROUND
        private void ExtractResource()
        {
            parentCell.resourceDeposits[0].quantity--;
            var resourceDeposit = parentCell.resourceDeposits[0];
            machine.outputInventory.Deposit(resourceDeposit.resource);
            // var instance = ResourceFactory.SpawnResource(resourceDeposit.material, resourceDeposit.type, Position);
            // if (instance is null) Debug.Log("NO PREFAB!");
            // return instance;
        }
    }
}
