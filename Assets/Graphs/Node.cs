using System.Collections.Generic;
using UnityEngine;
using ProcessControl.Industry;
using ProcessControl.Procedural;
#pragma warning disable 108,114


namespace ProcessControl.Graphs
{
    abstract public class Node : Entity, IO
    {
        // new public bool enabled;
        
        //> PROPERTIES
        public Cell parentCell;

        public Vector3 position
        {
            get => transform.position;
            set => transform.position = value;
        }
        
        //> HELPER FUNCTIONS
        public float DistanceTo(Node otherNode) => Vector3.Distance(this.position, otherNode.position);
        public Vector3 VectorTo(Node otherNode) => otherNode.position - this.position;
        public Vector3 DirectionTo(Node otherNode) => (otherNode.position - this.position).normalized;
        public static Vector3 Center(Node first, Node second) => (first.position + second.position) / 2f;
        public static float DistanceBetween(Node first, Node second) => Vector3.Distance(first.position, second.position);

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

        //> DELETE THIS NODE
        virtual public void OnDestroy()
        {
            if (parentCell is {}) parentCell.node = null;
            Destroy(gameObject);
        }
    }
}