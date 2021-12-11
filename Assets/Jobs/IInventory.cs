using ProcessControl.Industry;
using UnityEngine;


namespace ProcessControl.Jobs
{
    public interface IInventory
    {
        public Vector3 position {get;}
        
        public bool Contains(Stack stack);
        public Stack Withdraw(Stack stack);
        public void Deposit(Stack stack);
    }
}