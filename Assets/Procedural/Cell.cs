using System;
using System.Collections.Generic;
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
        public Resource.Material material;
        public Resource.Form type;
    }

    public class Cell
    {
        internal Chunk parentChunk;
        
        // might need to be converted to a bool
        public bool occupied => node is { };
        public bool buildable = true;

        public Vector3 position;
        public Vector2Int coords;

        public Biome biome;
        
        public Node node;
        public float terrainValue;
        public readonly Cell[] neighbours = new Cell[8];
        public readonly PathInfo pathInfo = new PathInfo();
        public readonly List<ResourceDeposit> resourceDeposits = new List<ResourceDeposit>();
    }

    public class PathInfo
    {
        public int hCost;
        public int gCost = int.MaxValue;
        public int fCost => gCost + hCost;

        public Cell previousInPath;

        public void Reset()
        {
            hCost = 0;
            gCost = int.MaxValue;
            previousInPath = null;
        }
    }
}