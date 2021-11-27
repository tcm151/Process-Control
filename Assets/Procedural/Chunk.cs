using System;
using UnityEngine;


namespace ProcessControl.Procedural
{
    [Serializable] public class Chunk
    {
        public Cell[,] cells;
        public Vector3 chunkCenter;
        public Vector2Int chunkOffset;
        
        public Chunk[] neighbours = new Chunk[8];
    }
}