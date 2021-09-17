using UnityEngine;
using ProcessControl.Tools;


namespace ProcessControl.Machines
{
    public class Extractor : Machine
    {
        [SerializeField] public Resource extractionResource;
        
        [Range(0, 64)] public float extractionSpeed;
        [SerializeField] public int inventorySize = 64;


        override public int InventorySize => inventorySize;
        override public bool Full => (machine.inventory.Count >= InventorySize);
        
        override protected void FixedUpdate()
        {
            base.FixedUpdate();
            
            if (++machine.ticks % (TicksPerSecond / extractionSpeed) == 0)
            {
                if (Full) return;
                
                machine.ticks = 0;
                if ((!machine.output || machine.output.Full) && !Full) Deposit(Extract());
                else if (!machine.output.Full)
                {
                    if (machine.inventory.Count >= 0) machine.output.Deposit(Withdraw());
                    else machine.output.Deposit(Extract());
                }
            }
        }

        protected Resource Extract()
        {
            var resource = Factory.Spawn("Resources", extractionResource, Position);
            // do more stuff
            return resource;
        }
    }
}
