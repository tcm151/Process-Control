using System.Collections.Generic;
using UnityEngine;


namespace ProcessControl.Industry.Resources
{
    [CreateAssetMenu(menuName = "Resources/Recipe")]
    public class Recipe : ScriptableObject
    {
        new public string name;
        
        public List<Resource> requiredResources;

        public Resource result;
    }
}