
using System.Collections.Generic;
using ProcessControl.Graphs;


namespace ProcessControl.Machines
{
    public interface IO<T>
    {
        public T Input {get;}
        public T Output {get;}
        
        public bool Full {get;}
        public bool Empty {get;}
        public int InventorySize {get;}
        
        public void ConnectInput(T input);
        public void ConnectOutput(T output);

        public Resource Withdraw();
        public void Deposit(Resource resource);
    }
}