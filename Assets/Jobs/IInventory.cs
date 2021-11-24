using ProcessControl.Industry;


namespace ProcessControl.Jobs
{
    public interface IInventory
    {
        public bool Contains(ItemAmount itemAmount);
        public ItemAmount Withdraw(ItemAmount itemAmount);
        public void Deposit(ItemAmount itemAmount);
    }
}