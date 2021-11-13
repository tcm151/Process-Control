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
        public Task Deconstruct(float deconstructionTime);
    }

    public interface IInventory
    {
        public bool Contains(ItemAmount itemAmount);
        public ItemAmount Withdraw(ItemAmount itemAmount);
        public void Deposit(ItemAmount itemAmount);
    }

    public interface IRepairable
    {
        public void Repair();
    }
}