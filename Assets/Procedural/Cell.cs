using System;
using System.Collections.Generic;
using UnityEngine;
using ProcessControl.Graphs;
using ProcessControl.Industry.Resources;
using UnityEngine.Serialization;


namespace ProcessControl.Procedural
{
    [Serializable] public class Cell
    {
        public bool occupied => node is { };
        public bool buildable = true;

        public Node node;
        public float terrainValue;
        public float resourceValue;

        public Resource.Type resourceType = Resource.Type.Copper;
        public int resourceDeposit;        

        public Vector3 position;
        public Vector2Int coordinates;
    }
}