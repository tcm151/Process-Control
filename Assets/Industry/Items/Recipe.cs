using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;


namespace ProcessControl.Industry.Resources
{
    [CreateAssetMenu(menuName = "Resources/Recipe")]
    public class Recipe : ScriptableObject
    {
        new public string name;
        
        public List<Item> requiredItems;
        public List<Item> resultingItems;
    }

    [Serializable] public class RecipeItem
    {
        public int amount;
        [FormerlySerializedAs("requiredItem")] public Item item;
    }
}