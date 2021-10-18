using UnityEngine;
using ProcessControl.Industry;
using ProcessControl.Industry.Resources;


namespace ProcessControl.Graphs
{
    abstract public class Edge : MonoBehaviour, IO
    {
        protected const int TicksPerSecond = 64;
        protected static int TicksPerMinute => TicksPerSecond * 60;
        
        abstract public float Length {get;}

        abstract public IO Input {get;}
        abstract public IO Output {get;}
        
        abstract public bool ConnectInput(IO input);
        abstract public bool DisconnectInput(IO input);
        abstract public bool ConnectOutput(IO output);
        abstract public bool DisconnectOutput(IO output);

        abstract public bool CanDeposit {get;}
        abstract public void Deposit(Entity entity);
        
        abstract public bool CanWithdraw {get;}
        abstract public Entity Withdraw();
        
        virtual public void OnDestroy() => Destroy(gameObject);
    }
}