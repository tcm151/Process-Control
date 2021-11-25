using ProcessControl.Industry;
using UnityEngine;


namespace ProcessControl.Jobs
{
    public interface IInventory
    {
        public Vector3 position {get;}
        
        public bool Contains(ItemAmount itemAmount);
        public ItemAmount Withdraw(ItemAmount itemAmount);
        public void Deposit(ItemAmount itemAmount);
    }
}