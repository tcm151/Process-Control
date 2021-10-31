using Aseprite;
using ProcessControl.Industry.Resources;
using UnityEngine;

namespace ProcessControl
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
