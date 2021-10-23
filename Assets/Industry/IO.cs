using ProcessControl.Industry.Conveyors;
using ProcessControl.Industry.Resources;


namespace ProcessControl.Industry
{
    public interface IO
    {
        public IO Input {get;}
        public IO Output {get;}

        public bool ConnectInput(IO newInput);
        public bool DisconnectInput(IO newInput);
        public bool ConnectOutput(IO newOutput);
        public bool DisconnectOutput(IO newOutput);

        public bool CanWithdraw();
        public Container Withdraw();

        public bool CanDeposit(Item item);
        public void Deposit(Container container);
    }
    
    public interface IInput
    {
        public bool ConnectOutput(IOutput newOutput);
        public bool DisconnectOutput(IOutput oldOutput);

        public bool CanDeposit(Container container);
        public void Deposit(Container container);
    }
    
    public interface IOutput
    {
        public bool ConnectInput(IInput newInput);
        public bool DisconnectInput(IInput oldInput);

        public bool CanWithdraw();
        public Container Withdraw();
    }
}