using System;
using System.Collections.Generic;
using ProcessControl.Machines;
using UnityEngine;
using Grid = ProcessControl.Terrain.Grid;


namespace ProcessControl.Machines
{
    abstract public class Node : Entity
    {
        //> NODE DATA CONTAINER
        [Serializable] public class Data
        {
            // public Grid.Cell cell;
            public List<Edge> edges;
        }

        [SerializeField] public Data node;
        
        //> PROPERTIES
        virtual public bool Full => false;
        public Vector3 Position => transform.position;
        // public Vector2Int Coordinates => node.cell.coordinates;
        
        //> DISTANCE BETWEEN NODES
        public float DistanceTo(Node otherNode) => Vector3.Distance(this.Position, otherNode.Position);
        public static float DistanceBetween(Node one, Node two) => Vector3.Distance(one.Position, two.Position);
        
        //> INITIALIZATION
        virtual protected void Awake() => node = new Data
        {
            // cell = null,
            edges = new List<Edge>(),
        };

        //> DELETE THIS NODE AND REMOVE ALL CONNECTIONS
        override public void Delete()
        {
            node.edges.ForEach(Destroy);
            Destroy(this.gameObject);
            node = null;
        }

        //> ADD A NEW NODE CONNECTION
        virtual public bool ConnectEdge(Edge newEdge)
        {
            if (node.edges.Contains(newEdge)) return false;
            node.edges.Add(newEdge);
            return true;
        }
        
        //> REMOVE A NODE CONNECTION
        virtual public bool RemoveConnection(Edge oldEdge)
        {
            if (!node.edges.Contains(oldEdge)) return false;
            node.edges.Remove(oldEdge);
            return true;
        }

        abstract public void Deposit(Resource resource);
        abstract public Resource Withdraw();
    }
}