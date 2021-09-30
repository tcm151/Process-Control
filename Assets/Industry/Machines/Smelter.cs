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

        private Resource Smelt(Resource resource)
        {
            // var gameObject = resource.gameObject;
            // var resourceData = resource.data;
            // resource.Remove();
            //
            // var ingot = gameObject.AddComponent<Ingot>();
            // ingot.data = resourceData;
            
            // return ingot;

            return resource;
        }

        // override public Resource Withdraw() => null;
    }
}