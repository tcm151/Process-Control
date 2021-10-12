using System;
using System.Collections.Generic;
using UnityEngine;
using ProcessControl.Graphs;
using ProcessControl.Industry.Resources;
using UnityEngine.Serialization;


namespace ProcessControl.Procedural
{
    public enum Direction { Up, Down, Left, Right }
    
    [Serializable] public class ResourceDeposit
    {
        [HideInInspector] public float noiseValue;
        
        public int quantity;
        public ResourceProperties.Material material;
        public ResourceProperties.Form type;
    }

    [Serializable] public class Cell
    {
        public enum Directions
        {
            UpLeft, Up, UpRight, Right,
            DownRight, Down, DownLeft, Left,
        }
        
        public bool occupied => node is { };
        public bool buildable = true;

        public Node node;
        public float terrainValue;
        // public Cell[] neighbours = new Cell[8];

        public Cell upLeft;
        public Cell up;
        public Cell upRight;
        public Cell right;
        public Cell downRight;
        public Cell down;
        public Cell downLeft;
        public Cell left;

        public List<ResourceDeposit> resourceDeposits = new List<ResourceDeposit>();

        public Vector3 position;
        public Vector2Int coords;
    }
}