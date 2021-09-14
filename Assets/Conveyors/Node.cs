using System;
using UnityEngine;


namespace ProcessControl.Conveyors
{
    public class Node : MonoBehaviour
    {
        public Node previous, next;
        
        private Vector3 position => transform.position;

        private void OnDrawGizmos()
        {
            if (next)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(position, next.position);
            }
        }
    }
}
