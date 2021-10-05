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
                
                
                var resource = machine.inputInventory.TakeFirst();
                var plate = EngagePress(resource);
                machine.outputInventory.Add(plate);
            }
        }

        private Resource EngagePress(Resource resource)
        {
            var instance = ResourceFactory.SpawnResource(resource.data.material, Resource.Type.Plate, Position);
            if (instance is null) Debug.Log("NO PREFAB!");
            instance.name = $"{instance.data.material} {instance.data.type}.{++Resource.Count:D4}";
            
            Destroy(resource);

            return instance;
        }
    }
}