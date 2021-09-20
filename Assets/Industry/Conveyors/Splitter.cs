using ProcessControl.Tools;


namespace ProcessControl.Machines
{
    class Splitter : Machine
    {
        override public bool Full => machine.outputs.Count == 0 || machine.currentOutput.Full || machine.inputInventory.Count >= InventorySize;
        override public bool Empty => machine.outputs.Count == 0 || machine.currentOutput.Full || machine.outputInventory.Count >= InventorySize;
        
        override public void Deposit(Resource resource)
        {
            resource.data.position = Position;
            machine.outputInventory.Add(resource);
        }

        override public Resource Withdraw()
        {
            // while (IOutput.Full) NextOutput();
            var resource = machine.outputInventory.TakeFirst();
            machine.currentOutput.Deposit(resource);
            NextOutput();
            return null;
        }
    }
}