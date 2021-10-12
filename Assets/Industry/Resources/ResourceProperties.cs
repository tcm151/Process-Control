using UnityEngine;
using UnityEngine.Serialization;


namespace ProcessControl.Industry.Resources
{
    [CreateAssetMenu(menuName = "Resources/Resource")]
    public class ResourceProperties : ScriptableObject
    {
        public enum Material { Copper, Iron, Gold, Platinum, Coal, Stone, Sand, }
        public enum Form { Raw, Ingot, Plate, Gear, Wire, Cable, Screw, }
    
        public Material material;
        public Form form;
        public Sprite sprite;
        
        new public string name => $"{material} {form}";
        [TextArea] public string description;
    }
}