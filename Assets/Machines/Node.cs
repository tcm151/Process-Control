using System;
using System.Collections.Generic;
using UnityEngine;
using Grid = ProcessControl.Terrain.Grid;


namespace ProcessControl.Conveyors
{
    abstract public class Node : MonoBehaviour
    {
        public Action<Node> OnAddConnection;
        
        virtual public bool Full => false;
        public Vector3 Position => transform.position;
        public Vector2Int Coordinates => nodeData.cell.coordinates;

        [Serializable] public class Data
        {
            public Grid.Cell cell;
            public List<Node> connections;
        }

        [SerializeField] protected Data nodeData;

        //> INIALIZATION
        virtual protected void Awake() => nodeData = new Data
        {
            cell = null,
            connections = new List<Node>(),
        };

        public void Delete() => nodeData.connections.ForEach(node => node.nodeData.connections.Remove(this));
        
        public bool AddConnection(Node newNode)
        {
            if (newNode == this) return false;
            nodeData.connections.Add(newNode);
            return true;
        }
        

        virtual public void RemoveConnection(Node removedNode)
        {
            if (removedNode == this) return;
            nodeData.connections.Remove(removedNode);
        }

        //> DRAW HELPFUL GIZMOS
        private void OnDrawGizmos()
        {
            if (nodeData.connections is { } && nodeData.connections.Count >= 1)
            {
                nodeData.connections.ForEach(node => Gizmos.DrawLine(Position, node.Position));
            }
        }

        public float DistanceTo(Node otherNode) => Vector3.Distance(this.Position, otherNode.Position);
        public static float DistanceBetween(Node one, Node two) => Vector3.Distance(one.Position, two.Position);
    }
}