using System.Threading.Tasks;
using UnityEngine;
using ProcessControl.Industry;

namespace ProcessControl.Graphs
{
    abstract public class Edge : Entity, IO
    {
        protected const int TicksPerSecond = 64;
        protected static int TicksPerMinute => TicksPerSecond * 60;
        
        public async Task Build(int buildTime)
        {
            var time = 0f;
            while ((time += Time.deltaTime) < buildTime) await Task.Yield();
            // renderer.color = enabledColor;
            enabled = true;
        }
        
        public async Task Deconstruct(int deconstructionTime)
        {
            var time = 0f;
            while ((time += Time.deltaTime) < deconstructionTime) await Task.Yield();
            Destroy(this);
        }
        
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
        
        virtual public void OnDestroy() => Destroy(gameObject);
    }
}