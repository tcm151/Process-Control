using System;
using System.Collections.Generic;
using UnityEngine;


namespace ProcessControl.Conveyors
{
    public class Node : MonoBehaviour
    {
        [SerializeField] private List<Node> connectedNodes;

        private Vector3 position => transform.position;

        private void Awake()
        {
            connectedNodes = new List<Node>();
        }

        public void Insert(Resource item)
        {
            
        }

        public void AddConnection(Node newNode)
        {
            if (newNode == this) return;
            connectedNodes.Add(newNode);
        }

        public void RemoveConnection(Node oldNode)
        {
            if (oldNode == this) return;
            connectedNodes.Remove(oldNode);
        }

        public void Delete()
        {
            connectedNodes.ForEach(n => n.connectedNodes.Remove(this));
        }

        private void OnDrawGizmos()
        {
            if (connectedNodes is { } && connectedNodes.Count > 0)
            {
                connectedNodes.ForEach(n =>
                {
                    Gizmos.DrawLine(position, n.position);
                });
            }
        }
    }
}
