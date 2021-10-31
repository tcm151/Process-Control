using UnityEngine;
using UnityEngine.Serialization;


namespace ProcessControl.Industry.Resources
{
    [CreateAssetMenu(menuName = "Resources/Resource")]
    public class Resource : Item
    {
        public enum Material { Copper, Iron, Gold, Platinum, Coal, Stone, Sand, Wood }
        public enum Form { Raw, Ingot, Plate, Gear, Wire, Cable, Screw, }
    
        [Header("Resource")]
        public Material material;
        public Form form;
        
        // new public string name => $"{material} {form}";
        // public Sprite sprite;
        // [TextArea] public string description;
    }
}