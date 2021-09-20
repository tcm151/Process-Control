
using System.Collections.Generic;
using ProcessControl.Graphs;


namespace ProcessControl.Machines
{
    public interface IO<T>
    {
        public T Input {get;}
        public T Output {get;}

        public void ConnectInput(T input);
        public void ConnectOutput(T output);

        public Resource Withdraw();
        public void Deposit(Resource resource);
    }

    public interface IInput
    {
        public bool Full {get;}
        public IOutput Output {get;}
        public void ConnectOutput(IOutput newOutput);
        public Resource Withdraw();
    }
    
    public interface IOutput
    {
        public bool Empty {get;}
        public IInput Input {get;}
        public void ConnectInput(IInput newInput);
        public void Deposit(Resource resource);
    }
}