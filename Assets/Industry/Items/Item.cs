using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProcessControl
{
    public class Item : ScriptableObject
    {
        new public string name;
        public Sprite sprite;
        [TextArea] public string description;
        
        
    }
}
