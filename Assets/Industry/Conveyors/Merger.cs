using System;
using ProcessControl.Tools;


namespace ProcessControl.Machines
{
    public class Merger : Machine
    {
        override public bool Full => machine.outputInventory.Count < machine.inventorySize;
        override public bool Empty => machine.outputInventory.Count == 0;
        
        override public void Deposit(Resource resource)
        {
            resource.data.position = Position;
            machine.outputInventory.Add(resource);
            NextInput();
        }

        override public Resource Withdraw() => machine.outputInventory.TakeFirst();
    }
}