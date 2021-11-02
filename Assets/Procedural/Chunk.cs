using System;
using UnityEngine;


namespace ProcessControl.Procedural
{
    [Serializable] public class Chunk
    {
        public Vector2Int chunkOffset;
        public Cell[,] cells;
        
        public Chunk[] neighbours = new Chunk[8];
    }
}