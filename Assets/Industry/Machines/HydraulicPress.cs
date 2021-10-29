using ProcessControl.Industry.Resources;
using ProcessControl.Tools;
using UnityEngine;


namespace ProcessControl.Industry.Machines
{
    public class HydraulicPress : Machine
    {
        public float speed;

        override protected void FixedUpdate()
        {
            base.FixedUpdate();

            if ((++ticks % (TicksPerMinute / speed)) == 0)
            {
                ticks = 0;
                if (inputInventory.Count == 0) return;
                
                
                EngagePress();
            }
        }

        private Container EngagePress()
        {
            if (currentRecipe.requiredItems.TrueForAll(requiredItem => inputInventory.Contains(requiredItem)))
            {
                currentRecipe.requiredItems.ForEach(i => inputInventory.Withdraw(i));
                currentRecipe.resultingItems.ForEach(r => outputInventory.Deposit(r));
                
                
            }

            return null;
        }
    }
}