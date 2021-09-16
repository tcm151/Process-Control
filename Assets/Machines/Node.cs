using System;
using System.Collections.Generic;
using UnityEngine;
using ProcessControl.Machines;
using Grid = ProcessControl.Terrain.Grid;


namespace ProcessControl.Conveyors
{
    abstract public class Node : MonoBehaviour, IO
    {
        public Vector3 Position => transform.position;
        public Vector2Int Coordinates => nodeData.cell.coordinates;

        [Serializable] public class Data
        {
            public Grid.Cell cell;
            public List<Node> connections;
        }

        [SerializeField] private Data nodeData;

        //> INIALIZATION
        private void Awake() => nodeData = new Data
        {
            cell = null,
            connections = new List<Node>(),
        };

        public void Insert(Resource item) { }
        public void Delete() => nodeData.connections.ForEach(node => node.nodeData.connections.Remove(this));

        
        virtual public void ConnectInput(Node newNode)
        {
            if (newNode == this) return;
            nodeData.connections.Add(newNode);
        }

        virtual public void ConnectOutput(Node node)
        {
            if (node == this) return;
            nodeData.connections.Add(node);
        }

        virtual public void RemoveConnection(Node removedNode)
        {
            if (removedNode == this) return;
            nodeData.connections.Remove(removedNode);
        }

        abstract public void DepositResource(Resource resource);
        abstract public Resource WithdrawResource();

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