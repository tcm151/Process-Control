using System;
using UnityEngine;


namespace ProcessControl.Machines
{
    public class Resource : MonoBehaviour
    {
        [Serializable] public class Data
        {
            public int ticks;
            public Color color = Color.grey;
            
            public Vector3 position;
        }

        public Data data;
        
        private void Awake()
        {
            data.position = transform.position;
        }

        private void FixedUpdate()
        {
            transform.position = data.position;
        }
        
        public void OnDrawGizmos()
        {
            Gizmos.color = data.color;
            Gizmos.DrawCube(data.position, 0.25f * Vector3.one);
        }
    }
}