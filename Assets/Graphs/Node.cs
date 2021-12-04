using System.Collections.Generic;
using UnityEngine;
using ProcessControl.Industry;
using ProcessControl.Procedural;
#pragma warning disable 108,114


namespace ProcessControl.Graphs
{
    abstract public class Node : Entity
    {
        //> PROPERTIES
        public Cell parentCell;

        // abstract public IO Input {get;}
        // abstract public IO Output {get;}
        //
        // abstract public bool ConnectInput(IO input);
        // abstract public bool DisconnectInput(IO input);
        // abstract public bool ConnectOutput(IO output);
        // abstract public bool DisconnectOutput(IO output);
        //
        // abstract public bool CanDeposit(Item item);
        // abstract public void Deposit(Container container);
        //
        // abstract public bool CanWithdraw();
        // abstract public Container Withdraw();

        //> DELETE THIS NODE
        virtual public void OnDestroy()
        {
            if (parentCell is {}) parentCell.node = null;
            Destroy(gameObject);
        }
    }
}