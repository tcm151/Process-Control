using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ProcessControl.Industry;


namespace ProcessControl.Jobs
{
    public interface Buildable
    {
        // public event Action onAllItemsDelivered;
        
        public Task DeliverItems(List<Stack> itemAmounts);
        public Task Build(float buildTime);
        public Task Disassemble(float deconstructionTime);
    }
}