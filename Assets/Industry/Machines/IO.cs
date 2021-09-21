
using System.Collections.Generic;
using ProcessControl.Industry.Resources;


namespace ProcessControl.Industry.Machines
{
    public interface IO
    {
        public IO Input {get;}
        public IO Output {get;}

        public bool ConnectInput(IO newInput);
        public bool ConnectOutput(IO newOutput);
        
        public bool CanWithdraw {get;}
        public Resource Withdraw();
        
        public bool CanDeposit {get;}
        public void Deposit(Resource resource);
    }
    
    public interface IInput
    {
        public bool ConnectOutput(IOutput newOutput);
        
        public bool CanDeposit {get;}
        public void Deposit(Resource resource);
    }
    
    public interface IOutput
    {
        public bool ConnectInput(IInput newInput);
        
        public bool CanWithdraw {get;}
        public Resource Withdraw();
    }
}