using System;
using UnityEngine;
using System.Collections.Generic;
using ProcessControl.Tools;


namespace ProcessControl.Procedural
{
    [Serializable] public class Chunk
    {
        public Vector2Int chunkOffset;
        public Cell[,] cells;
        public Range coordinateRange = new Range();
        
        public Chunk[] neighbours = new Chunk[8];
    }
}