using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ProcessControl.Graphs;
using ProcessControl.Industry.Resources;


namespace ProcessControl.Procedural
{
    public enum Biome { Empty, Grass, Forest, Ocean, Stone, Sand, Plains, Snow }
    
    [Serializable] public class ResourceDeposit
    {
        [HideInInspector] public float noiseValue;
        
        public int quantity;
        public Resource resource;
        // public Resource.Material material;
        // public Resource.Form type;
    }

    public class Cell
    {
        internal Chunk parentChunk;
        
        // might need to be converted to a bool
        public bool occupied => node is { } || edges.Count >= 1;
        public bool buildable = true;

        public Vector3 position;
        public Vector2Int coords;

        public Biome biome;
        
        public Node node;
        public readonly List<Edge> edges = new List<Edge>();
        public float terrainValue;
        public readonly Cell[] neighbours = new Cell[8];
        public readonly PathInfo pathInfo = new PathInfo();
        public readonly List<ResourceDeposit> resourceDeposits = new List<ResourceDeposit>();
    }

    public class PathInfo
    {
        public float gCost = float.MaxValue;
        public float hCost;
        public float fCost;

        public Cell previousInPath;

        public void Set(float g, float h, Cell previousCell)
        {
            gCost = g;
            hCost = h;
            fCost = gCost + hCost;
            previousInPath = previousCell;
        }
        
        public void Reset()
        {
            hCost = 0;
            gCost = int.MaxValue;
            previousInPath = null;
        }
    }
}