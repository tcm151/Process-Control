using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;


namespace ProcessControl.Industry
{
    [CreateAssetMenu(menuName = "Resources/Recipe")]
    public class Recipe : ScriptableObject
    {
        new public string name;
        public int energyCost;
        public int assemblyTime;
        [FormerlySerializedAs("requiredItems")] public List<Stack> inputItems;
        [FormerlySerializedAs("resultingItems")] public List<Stack> outputItems;
    }
}