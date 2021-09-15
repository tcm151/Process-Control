using UnityEngine;
using ProcessControl.Conveyors;

namespace ProcessControl
{
    public class Extractor : Machine
    {
        public float extractionSpeed;

        public Node output;

        private int ticks;
        
        private void FixedUpdate()
        {
            ticks++;
            
            if (!output) return;

            if (ticks > extractionSpeed * 64)
            {
                // connectedNodes[0].Insert(new Resource());
            }
        }
    }
}
