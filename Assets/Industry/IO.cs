

using UnityEngine;


namespace ProcessControl.Industry
{
    public interface IO
    {
        public Vector3 position {get;}
        
        public IO Input {get; set;}
        public IO Output {get; set;}

        public Inventory InputInventory {get;}
        public Inventory OutputInventory {get;}
        
        public bool ConnectInput(IO newInput);
        public bool DisconnectInput(IO newInput);
        public bool ConnectOutput(IO newOutput);
        public bool DisconnectOutput(IO newOutput);

        public bool CanWithdraw();
        public Container Withdraw();

        public bool CanDeposit(Item item);
        public void Deposit(Container container);
    }

    public static class IoExtensions
    {
        // public static bool ConnectInput(this IO source, IO destination)
        // {
        //     if (source.Output == destination) return false;
        //     source.Input = destination;
        //     // source.UpdateConnections();
        //     return true;
        // }
        //
        // public static bool ConnectOutput(this IO source, IO destination)
        // {
        //     if (output != oldOutput) return false;
        //     output = null;
        //     UpdateConnections();
        //     return true;
        // }
    }
}