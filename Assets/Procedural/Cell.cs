using System.Collections.Generic;
using UnityEngine;
using ProcessControl.Graphs;


namespace ProcessControl.Procedural
{
    public class Cell
    {
        public enum Direction
        {
            NorthWest, North, NorthEast, West, East, SouthWest, South, SouthEast,
        }

        internal Chunk parentChunk;
        
        public Node node;
        public readonly List<Edge> edges = new List<Edge>();
        
        public bool occupied => (node is {}) || (edges.Count >= 1);
        public bool walkable => buildable && (node is null || node is {enabled: false});
        public bool buildable = true;

        public Vector3 position;
        public Vector2Int coords;

        public Biome biome;
        
        public float terrainValue;
        public readonly Cell[] neighbours = new Cell[8];
        public readonly List<ResourceDeposit> resourceDeposits = new List<ResourceDeposit>();
        
        public readonly PathInfo pathInfo = new PathInfo();
    }
}