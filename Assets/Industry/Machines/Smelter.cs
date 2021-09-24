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
            
            if (++machine.ticks % (TicksPerSecond *  smeltingSpeed) == 0)
            {
                if (machine.inputInventory.Count == 0) return;
                
                machine.ticks = 0;
                

                var newResource = Smelt(machine.inputInventory.TakeFirst());
                Deposit(newResource);
                
                // if (!machine.currentOutput || !machine.currentOutput.CanDeposit)
                // {
                //     machine.outputInventory.Add(newResource);
                // }
                // else
                // {
                //     newResource.SetVisible(true);
                //     machine.currentOutput.Deposit(newResource);
                // }
            }
        }

        private Resource Smelt(Resource resource)
        {
            // Debug.Log("SMELTED RESOURCE!");
            return resource;
        }

        override public Resource Withdraw() => null;
    }
}