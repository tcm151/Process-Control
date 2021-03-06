using UnityEngine;


namespace ProcessControl.Industry
{
    [CreateAssetMenu(menuName = "Resources/Resource")]
    public class Resource : Item
    {
        public enum Material { Copper, Iron, Gold, Platinum, Coal, Stone, Sand, Wood }
        public enum Form { Raw, Ingot, Plate, Gear, Wire, Cable, Screw, }
    
        [Header("Resource")]
        public Material material;

        public float energy;
        public float burnTime;
        public float energyPerTick;
    }
}