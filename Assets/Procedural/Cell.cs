using System;
using System.Collections.Generic;
using UnityEngine;
using ProcessControl.Graphs;
using ProcessControl.Industry.Resources;


namespace ProcessControl.Procedural
{
    [Serializable] public class Cell
    {
        public void ResetNode() => node = null;

        public Node node;
        public bool occupied => node is { };

        public float value;

        public Vector3 center;
        public Vector2Int coordinates;

        public List<(int, Resource.Type)> resourceDeposit = new List<(int, Resource.Type)>();
    }
}