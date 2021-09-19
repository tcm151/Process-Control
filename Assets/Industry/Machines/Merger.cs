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
            // if (machine.outputs is {} && !machine.outputs.Full) machine.outputs.Deposit(resource);
            machine.inventory.Add(resource);
            NextInput();
            // machine.outputs.Deposit(Withdraw());
        }

        override public Resource Withdraw() => machine.inventory.TakeFirst();
    }
}