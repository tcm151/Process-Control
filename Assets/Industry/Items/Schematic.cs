using ProcessControl.Graphs;
using UnityEngine;


namespace ProcessControl.Industry
{
    [CreateAssetMenu(menuName = "Resources/Part")]
    public class Schematic : Item
    {
        // [Header("Information")]
        // new public string name;
        // public Recipe recipe;
        // public Sprite sprite;
        // [TextArea] public string description;
        public Entity entity;
    }
}