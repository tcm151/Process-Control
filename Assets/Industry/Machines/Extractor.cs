using ProcessControl.Industry.Resources;
using UnityEngine;
using ProcessControl.Tools;


namespace ProcessControl.Industry.Machines
{
    public class Extractor : Machine
    {
        [SerializeField] public Resource extractionResource;
        [Range(0, 64)] public float extractionSpeed;

        
        override protected void FixedUpdate()
        {
            base.FixedUpdate();
            if (parentCell.resourceDeposit <= 0)
            {
                machine.sleeping = true;
                return;
            }
            
            if ((++machine.ticks % (TicksPerSecond / extractionSpeed)) == 0)
            {
                if (machine.outputInventory.Count >= machine.inventorySize) return;
                
                machine.ticks = 0;
                Deposit(ExtractResource());
            }
        }

        override public void Deposit(Resource resource)
        {
            resource.data.position = Position;
            // node.inventory.Add(resource);
            machine.outputInventory.Add(resource);
            resource.SetVisible(false);
            // NextInput();
        }

        private int i;

        protected Resource ExtractResource()
        {
            parentCell.resourceDeposit--;
            var resource = Factory.Spawn("Resources", extractionResource, Position);
            resource.data.type = parentCell.resourceType;
            resource.name = $"{resource.data.type}.{i++:D3}";
            return resource;
        }
    }
}
