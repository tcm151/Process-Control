using ProcessControl.Graphs;
using ProcessControl.Industry.Machines;
using ProcessControl.Industry.Resources;


namespace ProcessControl.Industry.Conveyors
{
    abstract public class Junction : Node
    {
        abstract override public IO Input {get;}
        abstract override public IO Output {get;}
        
        abstract override public bool ConnectInput(IO input);
        abstract override public bool DisconnectInput(IO input);
        abstract override public bool ConnectOutput(IO output);
        abstract override public bool DisconnectOutput(IO output);
        
        abstract override public bool CanDeposit {get;}
        abstract override public void Deposit(Entity entity);
        
        abstract override public bool CanWithdraw {get;}
        abstract override public Entity Withdraw();
    }
}