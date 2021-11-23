using ProcessControl.Industry;


namespace ProcessControl.Graphs
{
    abstract public class MultiNode : Node
    {

        abstract override public IO Input {get;}
        abstract override public IO Output {get;}
        
        abstract override public bool ConnectInput(IO input);
        abstract override public bool DisconnectInput(IO input);
        abstract override public bool ConnectOutput(IO output);
        abstract override public bool DisconnectOutput(IO output);

        abstract override public bool CanDeposit(Item item);
        abstract override public void Deposit(Container container);

        abstract override public bool CanWithdraw();
        abstract override public Container Withdraw();
    }
}