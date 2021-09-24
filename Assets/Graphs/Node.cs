using ProcessControl.Industry.Machines;
using UnityEngine;
using ProcessControl.Procedural;
using ProcessControl.Industry.Resources;


namespace ProcessControl.Graphs
{
    abstract public class Node : MonoBehaviour, IO
    {
        protected const int TicksPerSecond = 64;
        
        //> PROPERTIES
        public ProceduralGrid.Cell Cell {get; set;}
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
        
        abstract public bool CanDeposit {get;}
        abstract public void Deposit(Resource resource);
        
        abstract public bool CanWithdraw {get;}
        abstract public Resource Withdraw();

        //> DELETE THIS NODE
        virtual public void OnDestroy() => Destroy(gameObject);
    }
}