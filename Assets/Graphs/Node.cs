using UnityEngine;
using ProcessControl.Industry;
using ProcessControl.Industry.Resources;
using ProcessControl.Procedural;
#pragma warning disable 108,114


namespace ProcessControl.Graphs
{
    abstract public class Node : MonoBehaviour, IO
    {
        protected const int TicksPerSecond = 64;
        protected static int TicksPerMinute => TicksPerSecond * 60;
        
        new public bool enabled;
        
        public Color enabledColor = new Color(255, 255, 255, 255);
        public Color disabledColor = new Color(255, 255, 255, 100);
        
        //> PROPERTIES
        public Cell parentCell;
        public Vector3 Position => transform.position;
        
        //> INSTANCE HELPERS
        public float DistanceTo(Node otherNode) => Vector3.Distance(this.Position, otherNode.Position);
        public Vector3 VectorTo(Node otherNode) => otherNode.Position - this.Position;
        public Vector3 DirectionTo(Node otherNode) => (otherNode.Position - this.Position).normalized;
        
        //> STATIC HELPERS
        public static Vector3 Center(Node first, Node second) => (first.Position + second.Position) / 2f;
        public static float DistanceBetween(Node first, Node second) => Vector3.Distance(first.Position, second.Position);

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
            parentCell.node = null;
            Destroy(gameObject);
        }
    }
}