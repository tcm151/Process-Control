namespace ProcessControl.Machines
{
    public class TransportNode : Machine
    {
        override public bool Full => !machine.output || machine.output.Full;
        
        override public void Deposit(Resource resource)
        {
            resource.data.position = Position;
            machine.output.Deposit(resource);
        }

        override public Resource Withdraw() => machine.input.Withdraw();
    }
}