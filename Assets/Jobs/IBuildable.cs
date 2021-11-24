using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ProcessControl.Industry;


namespace ProcessControl.Jobs
{
    public interface IBuildable
    {
        // public event Action onAllItemsDelivered;
        
        public Task DeliverItems(List<ItemAmount> itemAmounts);
        public Task Build(float buildTime);
        public Task Disassemble(float deconstructionTime);
    }
}