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
                machine.ticks = 0;
                var newResource = Smelt(machine.inventory.TakeFirst());

                Destroy(newResource.gameObject);
                Debug.Log("SMELTED RESOURCE!");
            }
        }

        private Resource Smelt(Resource resource)
        {
            return resource;
        }
    }
}