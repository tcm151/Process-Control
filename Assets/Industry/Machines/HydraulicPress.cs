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
                if (plate is null) return;
                machine.outputInventory.Add(plate);
            }
        }

        private Entity EngagePress(Entity entity)
        {
            if (entity.item is Resource resource)
            {
                var instance = ResourceFactory.SpawnResource(resource.material, Resource.Form.Plate, Position);
                if (instance is null) Debug.Log("NO PREFAB!");
                instance.name = $"{resource.material} {resource.form}.{++Entity.Count:D4}";
                
                Destroy(entity);

                return instance;
            }

            return null;
        }
    }
}