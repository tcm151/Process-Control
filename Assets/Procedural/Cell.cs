using System;
using System.Collections.Generic;
using UnityEngine;
using ProcessControl.Graphs;
using ProcessControl.Industry.Resources;


namespace ProcessControl.Procedural
{
    [Serializable] public class Cell
    {
        public bool occupied => node is { };
        public bool buildable = true;

        public Node node;
        public float value;

        public Vector3 position;
        public Vector2Int coordinates;
    }
}