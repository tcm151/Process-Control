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
                
                
                // var resource = machine.inputInventory.Withdraw();
                EngagePress();
                // if (plate is null) return;
                // machine.outputInventory.Deposit(plate);
            }
        }

        private Container EngagePress()
        {
            if (machine.currentRecipe.requiredItems.TrueForAll(requiredItem => machine.inputInventory.Has(requiredItem, 1)))
            // if (container.item is Resource resource)
            {
                var inputItems = machine.inputInventory.Withdraw(1);
                // var instance = ResourceFactory.SpawnResource(resource.material, Resource.Form.Plate, Position);
                // if (instance is null) Debug.Log("NO PREFAB!");
                // instance.name = $"{resource.material} {resource.form}.{++Container.Count:D4}";
                machine.currentRecipe.resultingItems.ForEach(r => machine.outputInventory.Deposit(r));
                
                
                // inputItems.ForEach(Destroy);
                // Destroy(container);

                // return instance;
            }

            return null;
        }
    }
}