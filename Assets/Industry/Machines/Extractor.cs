using ProcessControl.Industry.Resources;
using UnityEngine;
using ProcessControl.Tools;


namespace ProcessControl.Industry.Machines
{
    public class Extractor : Machine
    {
        public ResourceFactory resourceFactory;
        
        [Range(0, 64)] public float extractionSpeed;

        //> FIXED CALCULATION INTERVAL
        override protected void FixedUpdate()
        {
            base.FixedUpdate();
            
            // sleep if ground is depleted
            if (parentCell.resourceDeposits.Count == 0 || parentCell.resourceDeposits[0].quantity <= 0)
            {
                machine.sleeping = true;
                return;
            }
            
            // extraction interval check
            if ((++machine.ticks % (TicksPerSecond / extractionSpeed)) == 0)
            {
                if (machine.outputInventory.Count >= machine.inventorySize) return;
                
                machine.ticks = 0;
                Deposit(ExtractResource());
            }
        }

        //> DEPOSIT RESOURCE INTO INVENTORY
        override public void Deposit(Resource resource)
        {
            resource.position = Position;
            machine.outputInventory.Add(resource);
            resource.SetVisible(false);
        }

        private int i;

        //> EXTRACT RESOURCE FROM THE GROUND
        protected Resource ExtractResource()
        {
            parentCell.resourceDeposits[0].quantity--;
            // var resource = Factory.Spawn("Resources", extractionResource, Position);
            var prefab = parentCell.resourceDeposits[0];
            // Debug.Log(prefab.material);
            // Debug.Log(prefab.type);
            var instance = ResourceFactory.SpawnResource(prefab.material, prefab.type, Position);
            if (instance is null) Debug.Log("NO PREFAB!");
            instance.name = $"{instance.data.material} {instance.data.type}.{i++:D4}";
            return instance;
        }
    }
}
