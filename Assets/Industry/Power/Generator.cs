using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;


namespace ProcessControl.Industry.Power
{
    public class Generator : Machine
    {
        [Header("Generator")]
        public float energy;
        public float maxEnergy;
        public List<Resource> acceptedFuels;

        private bool burningFuel;

        virtual protected void FixedUpdate()
        {
            if (burningFuel) return;
            

            if (acceptedFuels.Any(fuel => inputInventory.Contains(fuel)))
            {
                var match = acceptedFuels.FirstOrDefault(f => inputInventory.Contains(f));
                
                if (match is null) return;
                if (energy > maxEnergy - match.energy) return;
                // Debug.Log("Can Burn Fuel");

                var fuel = inputInventory.Withdraw(match);
                BurnFuel(fuel as Resource);
            }
        }
        
        private async void BurnFuel(Resource fuel)
        {
            // Debug.Log("Burning Fuel");
            burningFuel = true;

            while (ticks++ < fuel.burnTime)
            {
                energy += fuel.energyPerTick;
                await Task.Yield();
            }

            ticks = 0;
            burningFuel = false;
            // Debug.Log("Done Burning..");
        }
    }
}