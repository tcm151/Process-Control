using System;
using ProcessControl.Machines;
using UnityEngine;


namespace ProcessControl.Graphs
{
    abstract public class Edge : MonoBehaviour
    {
        [Serializable]
        public class Data
        {
            public Node input;
            public Node output;
        }
        [SerializeField] internal Data edgeData;

        abstract public void Delete();

        virtual public float Length { get
        {
            if (!edgeData.input || !edgeData.output) return 0;
            return Node.DistanceBetween(edgeData.input, edgeData.output);
        }}
    }
}