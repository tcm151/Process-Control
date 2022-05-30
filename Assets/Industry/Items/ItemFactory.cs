using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using ProcessControl.Tools;


namespace ProcessControl.Industry
{
    public class ItemFactory : Factory
    {
        public string sceneName = "Items";
        [FormerlySerializedAs("container")] public Container containerPrefab;
        [SerializeField] public List<Item> itemPrefabs;
        [SerializeField] private List<Container> spawnedContainers = new List<Container>();

        //> EVENT HOOKS
        public static Func<string, Item> GetItem;
        public static Func<Stack, bool> Exists;
        public static Func<Vector3, Stack, Container> FindItemByClosest;
        public static Func<Vector3, Stack, List<Container>> FindItemsByClosest;
        public static Func<Item, Vector3, Container> SpawnContainer;
        public static Action<Container> DisposeContainer;

        private void Awake()
        {
            GetItem += (name) =>
            {
                var item = itemPrefabs.FirstOrDefault(i => i.name == name);
                if (item is {}) return item;
            
                Debug.Log($"Item \"{name}\" was not found.");
                return default;
            };

            Exists += (itemAmount) =>
            {
                var matchingContainers = spawnedContainers.Where(c => c.stack == itemAmount);
                return (matchingContainers.Count() >= itemAmount.amount);
            };

            FindItemsByClosest += (position, itemAmount) =>
            {
                var matchingContainers = spawnedContainers.Where(c => c.stack == itemAmount)
                                        .OrderBy(c => Vector3.Distance(c.position, position))
                                        .ToList();
                    
                return matchingContainers.Take(itemAmount.amount).ToList();
            };

            FindItemByClosest += (position, itemAmount) =>
            {
                var matchingContainer = spawnedContainers.Where(c => c.stack == itemAmount)
                                                         .OrderBy(c => Vector3.Distance(c.position, position))
                                                         .FirstOrDefault();
                return matchingContainer;
            };
            
            SpawnContainer += (item, position) =>
            {
                var instance = Spawn(sceneName, containerPrefab, position);
                instance.SetItem(new Stack{item = item, amount = 1});
                spawnedContainers.Add(instance);
                return instance;
            };

            DisposeContainer += (container) =>
            {
                spawnedContainers.Remove(container);
                Destroy(container);
            };
        }
    }
}