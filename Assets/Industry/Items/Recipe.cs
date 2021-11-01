﻿using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;


namespace ProcessControl.Industry.Resources
{
    [CreateAssetMenu(menuName = "Resources/Recipe")]
    public class Recipe : ScriptableObject
    {
        new public string name;
        public int energyCost;
        [FormerlySerializedAs("requiredItems")] public List<ItemAmount> inputItems;
        [FormerlySerializedAs("resultingItems")] public List<ItemAmount> outputItems;
    }

    [Serializable] public class ItemAmount
    {
        public Item item;
        public int amount = 1;
    }
}