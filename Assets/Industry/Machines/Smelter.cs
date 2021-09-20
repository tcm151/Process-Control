using ProcessControl.Tools;
using UnityEngine;


namespace ProcessControl.Machines
{
    public class Smelter : Machine
    {
        [Range(1, 64)] public float smeltingSpeed;

        override protected void FixedUpdate()
        {
            base.FixedUpdate();
            
            if (++machine.ticks % (TicksPerSecond *  smeltingSpeed) == 0)
            {
                if (Empty) return;
                
                machine.ticks = 0;
                var newResource = Smelt(machine.inputInventory.TakeFirst());


                if (!machine.currentOutput || machine.currentOutput.Full)
                {
                    machine.outputInventory.Add(newResource);
                }
                else
                {
                    newResource.SetVisible(true);
                    machine.currentOutput.Deposit(newResource);
                }
            }
        }

        private Resource Smelt(Resource resource)
        {
            // Debug.Log("SMELTED RESOURCE!");
            resource.SetColor(Color.red);
            return resource;
        }

        override public Resource Withdraw() => null;
    }
}