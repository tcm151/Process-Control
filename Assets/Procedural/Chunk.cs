using System;
using UnityEngine;
using System.Collections.Generic;
using ProcessControl.Tools;


namespace ProcessControl.Procedural
{
    [Serializable] public class Chunk
    {
        public Vector2Int chunkOffset;
        public Range noiseRange = new Range();
        public List<Cell> cells = new List<Cell>();
        
    }
}