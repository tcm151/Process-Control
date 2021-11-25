using System;
using ProcessControl.Industry;
using UnityEngine;


namespace ProcessControl.Procedural
{
    [Serializable] public class ResourceDeposit
    {
        [HideInInspector] public float noiseValue;
        
        public int quantity;
        public Resource resource;
    }
}