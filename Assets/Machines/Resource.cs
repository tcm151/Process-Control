using System;
using UnityEngine;


namespace ProcessControl.Conveyors
{
    public class Resource : MonoBehaviour
    {
        [Serializable] public class Data
        {
            public Color color = Color.grey;
            
            public Vector3 position;
            public Vector2Int coordinates;
        }

        public Data data;
        
        private void Awake()
        {
            data.position = transform.position;
        }

        public void SetCoordinates(Vector2Int newCoordinates)
        {
            data.coordinates = newCoordinates;
        }

        public void OnDrawGizmos()
        {
            Gizmos.color = data.color;
            Gizmos.DrawCube(data.position, 0.25f * Vector3.one);
        }
    }
}