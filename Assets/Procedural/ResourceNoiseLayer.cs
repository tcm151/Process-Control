using System;
using ProcessControl.Industry;


namespace ProcessControl.Procedural
{
    [Serializable] public class ResourceNoiseLayer : Noise.Layer
    {
        public Resource resource;
    }
}