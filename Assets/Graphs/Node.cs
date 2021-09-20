using System;
using System.Collections.Generic;
using ProcessControl.Machines;
using UnityEngine;
using Grid = ProcessControl.Terrain.Grid;


namespace ProcessControl.Graphs
{
    abstract public class Node : MonoBehaviour
    {
        //> NODE DATA CONTAINER
        [Serializable] public class Data
        {
            public Grid.Cell cell;
            public List<Edge> edges = new List<Edge>();
        }
        [SerializeField] internal Data node;

        //> EVENTS
        public event Action onConnectEdge;
        public event Action onDisconnectEdge;
        
        public Vector3 Position => transform.position;
        public Vector2Int Coordinates => node.cell.coordinates;
        
        //> DISTANCE BETWEEN NODES
        public float DistanceTo(Node otherNode) => Vector3.Distance(this.Position, otherNode.Position);
        public Vector3 VectorTo(Node otherNode) => otherNode.Position - this.Position;
        public Vector3 DirectionTo(Node otherNode) => (otherNode.Position - this.Position).normalized;
        
        //> STATIC HELPERS
        public static Vector3 Center(Node first, Node second) => (first.Position + second.Position) / 2f;
        public static float DistanceBetween(Node first, Node second) => Vector3.Distance(first.Position, second.Position);

        //> DELETE THIS NODE AND REMOVE ALL CONNECTIONS
        virtual public void OnDestroy()
        {
            node.edges.ForEach(Destroy);
            node.edges.Clear();
            node.cell.machine = null;
            Destroy(gameObject);
        }

        //> ADD A NEW NODE CONNECTION
        virtual public bool ConnectEdge(Edge newEdge)
        {
            if (node.edges.Contains(newEdge)) return false;
            node.edges.Add(newEdge);
            onConnectEdge?.Invoke();
            return true;
        }
        
        //> REMOVE A NODE CONNECTION
        virtual public bool DisconnectEdge(Edge oldEdge)
        {
            if (!node.edges.Contains(oldEdge)) return false;
            node.edges.Remove(oldEdge);
            onDisconnectEdge?.Invoke();
            return true;
        }
    }
}