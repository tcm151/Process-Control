using UnityEngine;

namespace ProcessControl.Industry
{
    public class Item : ScriptableObject
    {
        [Header("Information")]
        new public string name;
        public Recipe recipe;
        public Sprite sprite;
        [TextArea] public string description;
    }
}
