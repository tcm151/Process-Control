using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ProcessControl.Industry;
using UnityEngine.Serialization;


namespace ProcessControl.Tools
{
    public class ItemFactory : Factory
    {
        public string sceneName = "Items";
        [FormerlySerializedAs("container")] public Container containerPrefab;
        [SerializeField] public List<Item> itemPrefabs;
        [SerializeField] private List<Container> spawnedContainers = new List<Container>();

        //> EVENT HOOKS
        public static Func<string, Item> GetItem;
        public static Func<ItemAmount, bool> Exists;
        public static Func<Vector3, int, List<Container>> FindClosest;
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
                var matchingContainers = spawnedContainers.Where(c => c.item == itemAmount.item);
                return (matchingContainers.Count() >= itemAmount.amount);
            };

            FindClosest += (position, amount) =>
            {
                return spawnedContainers.OrderBy(c => Vector3.Distance(c.position, position))
                                        .Take(amount).ToList();
            };
            
            SpawnContainer += (item, position) =>
            {
                var instance = Spawn(sceneName, containerPrefab, position);
                instance.SetItem(item);
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