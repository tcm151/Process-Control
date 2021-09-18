using System;
using System.Collections.Generic;
using ProcessControl.Machines;
using UnityEngine;
using Grid = ProcessControl.Terrain.Grid;


namespace ProcessControl.Graphs
{
    abstract public class Node : Entity
    {
        //> NODE DATA CONTAINER
        [Serializable] public class Data
        {
            public Grid.Cell cell;
            public List<Edge> edges = new List<Edge>();
        }
        [SerializeField] private Data node;

        //> EVENTS
        virtual public event Action onConnectEdge;
        virtual public event Action onDisconnectEdge;
        
        //> PROPERTIES
        // abstract public bool Full {get;}
        // abstract public bool Empty {get;}
        // abstract public int InventorySize {get;}
        
        virtual public Vector3 Position => transform.position;
        virtual public Vector2Int Coordinates => node.cell.coordinates;
        
        //> DISTANCE BETWEEN NODES
        public float DistanceTo(Node otherNode) => Vector3.Distance(this.Position, otherNode.Position);
        public Vector3 VectorTo(Node otherNode) => otherNode.Position - this.Position;
        public Vector3 DirectionTo(Node otherNode) => (otherNode.Position - this.Position).normalized;
        
        //> STATIC HELPERS
        public static Vector3 Center(Node first, Node second) => (first.Position + second.Position) / 2f;
        public static float DistanceBetween(Node first, Node second) => Vector3.Distance(first.Position, second.Position);

        //> DELETE THIS NODE AND REMOVE ALL CONNECTIONS
        override public void Delete()
        {
            node.edges.ForEach(e => e.Delete());
            // node.cell.machine = null;
            Destroy(this.gameObject);
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