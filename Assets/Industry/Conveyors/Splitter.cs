using ProcessControl.Tools;


namespace ProcessControl.Machines
{
    class Splitter : Machine
    {
        override public bool Full => machine.outputs.Count == 0 || machine.currentOutput.Full || machine.inventory.Count >= InventorySize;
        
        override public void Deposit(Resource resource)
        {
            resource.data.position = Position;
            machine.inventory.Add(resource);
        }

        override public Resource Withdraw()
        {
            var resource = machine.inventory.TakeFirst();
            machine.currentOutput.Deposit(resource);
            NextOutput();
            return null;
        }
    }
}