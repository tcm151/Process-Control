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

        public static ItemFactory Instance {get; private set;}
        private void Awake()
        {
            Instance = this;
        }
        
        public Item GetItem(string name)
        {
            var item = itemPrefabs.FirstOrDefault(i => i.name == name);
            if (item is {}) return item;
            
            Debug.Log($"Item \"{name}\" was not found.");
            return default;
        }


        public bool Exists(ItemAmount itemAmount)
        {
            var matchingContainers = spawnedContainers.Where(c => c.item == itemAmount.item);
            return (matchingContainers.Count() >= itemAmount.amount);
        }
        
        public List<Container> FindClosest(Vector3 position, int amount = 1)
        {
            return spawnedContainers.OrderBy(c => Vector3.Distance(c.position, position)).Take(amount).ToList();
        }


        public Container SpawnContainer(Item item, Vector3 position)
        {
            var instance = Spawn(sceneName, containerPrefab, position);
            instance.SetItem(item);
            spawnedContainers.Add(instance);
            return instance;
        }

        public List<Container> SpawnContainers(List<Item> items, Vector3 position)
        {
            var instances = new List<Container>();
            items.ForEach(i => instances.Add(SpawnContainer(i, position)));
            instances.ForEach(i => spawnedContainers.Add(i));
            return instances;
        }

        public void DisposeContainer(Container container)
        {
            spawnedContainers.Remove(container);
            Destroy(container);
        }
    }
}