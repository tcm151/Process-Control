using UnityEngine;


namespace ProcessControl.Industry.Machines
{
    public class Extractor : Machine
    {
        [Range(0, 64)] public float extractionSpeed;

        //> FIXED CALCULATION INTERVAL
        override protected void FixedUpdate()
        {
            if (!enabled || sleeping) return;
            if (ticks >= sleepThreshold)
            {
                sleeping = true;
                return;
            }
            
            // sleep if ground is depleted
            if (parentCell.resourceDeposits.Count == 0 || parentCell.resourceDeposits[0].quantity <= 0)
            {
                sleeping = true;
                return;
            }
            
            // extraction interval check
            if (++ticks > (TicksPerMinute / extractionSpeed))
            {
                ticks = 0;
                if (outputInventory.Count >= inventorySize) return;

                ExtractResource();
            }
        }

        //> EXTRACT RESOURCE FROM THE GROUND
        private void ExtractResource()
        {
            // Debug.Log("Extracting Resource...");
            parentCell.resourceDeposits[0].quantity--;
            var resourceDeposit = parentCell.resourceDeposits[0];
            // Debug.Log($"Depositing {resourceDeposit.resource}");
            outputInventory.Deposit(resourceDeposit.resource);
        }
    }
}
