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
                
                
                var resource = machine.inputInventory.Withdraw();
                var plate = EngagePress(resource);
                if (plate is null) return;
                machine.outputInventory.Deposit(plate);
            }
        }

        private Container EngagePress(Container container)
        {
            if (container.item is Resource resource)
            {
                var instance = ResourceFactory.SpawnResource(resource.material, Resource.Form.Plate, Position);
                if (instance is null) Debug.Log("NO PREFAB!");
                instance.name = $"{resource.material} {resource.form}.{++Container.Count:D4}";
                
                Destroy(container);

                return instance;
            }

            return null;
        }
    }
}