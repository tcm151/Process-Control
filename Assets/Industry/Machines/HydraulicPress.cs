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

            if ((++machine.ticks % (TicksPerMinute / speed)) == 0)
            {
                machine.ticks = 0;
                if (machine.inputInventory.Count == 0) return;
                
                
                EngagePress();
            }
        }

        private Container EngagePress()
        {
            if (machine.currentRecipe.requiredItems.TrueForAll(requiredItem => machine.inputInventory.Contains(requiredItem)))
            {
                machine.currentRecipe.requiredItems.ForEach(i => machine.inputInventory.Withdraw(i));
                machine.currentRecipe.resultingItems.ForEach(r => machine.outputInventory.Deposit(r));
                
                
            }

            return null;
        }
    }
}