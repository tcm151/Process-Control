using System.Threading.Tasks;
using UnityEngine;
using ProcessControl.Industry;
using ProcessControl.Jobs;


namespace ProcessControl.Graphs
{
    abstract public class Edge : Entity, IO
    {
        abstract public float Length {get;}
        abstract public Vector3 Center {get;}

        abstract public IO Input {get;}
        abstract public IO Output {get;}
        
        abstract public bool ConnectInput(IO input);
        abstract public bool DisconnectInput(IO input);
        abstract public bool ConnectOutput(IO output);
        abstract public bool DisconnectOutput(IO output);

        abstract public bool CanDeposit(Item item);
        abstract public void Deposit(Container container);

        abstract public bool CanWithdraw();
        abstract public Container Withdraw();
    }
}