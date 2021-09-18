using UnityEngine;
using ProcessControl.Tools;


namespace ProcessControl.Machines
{
    public class Extractor : Machine
    {
        [SerializeField] public Resource extractionResource;
        [Range(0, 64)] public float extractionSpeed;
        
        override protected void FixedUpdate()
        {
            base.FixedUpdate();
            
            if (++machine.ticks % (TicksPerSecond / extractionSpeed) == 0)
            {
                if (Full) return;
                
                machine.ticks = 0;
                if (!Full) Deposit(Extract());
                // else if (!machine.output.Full)
                {
                    // if (machine.inventory.Count >= 0) machine.output.Deposit(Withdraw());
                    // machine.output.Deposit(Extract());
                }
            }
        }

        private int i;
        
        protected Resource Extract()
        {
            var resource = Factory.Spawn("Resources", extractionResource, Position);
            resource.name = $"Resource.{i++:D3}";
            // do more stuff
            return resource;
        }
    }
}
