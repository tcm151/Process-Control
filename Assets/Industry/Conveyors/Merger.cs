using System;
using ProcessControl.Tools;


namespace ProcessControl.Machines
{
    public class Merger : Machine
    {
        override public bool Full => machine.outputs.Count == 0 || machine.currentOutput.Full || machine.inventory.Count >= InventorySize;
        
        override public void Deposit(Resource resource)
        {
            resource.data.position = Position;
            machine.inventory.Add(resource);
            NextInput();
        }

        override public Resource Withdraw() => machine.inventory.TakeFirst();
    }
}