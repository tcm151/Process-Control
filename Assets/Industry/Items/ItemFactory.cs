using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ProcessControl.Industry;


namespace ProcessControl.Tools
{
    public class ItemFactory : Factory
    {
        public string sceneName = "Items";
        public Container container;
        [SerializeField] public List<Item> itemPrefabs;
        [SerializeField] private List<Container> spawnedContainers = new List<Container>();

        public Item GetItem(string name)
        {
            var item = itemPrefabs.FirstOrDefault(i => i.name == name);
            if (item is {}) return item;
            
            Debug.Log($"Item \"{name}\" was not found.");
            return default;
        }

        public Part GetPart(string name)
        {
            var item = GetItem(name);
            return (item is Part part) ? part : default;
        }
        
        private void Awake() => Instance = this;
        public static ItemFactory Instance {get; private set;}

        public Container SpawnItem(Item item, Vector3 position)
        {
            var instance = Spawn(sceneName, container, position);
            instance.SetItem(item);
            spawnedContainers.Add(instance);
            return instance;
        }

        public List<Container> SpawnItems(List<Item> items, Vector3 position)
        {
            var instances = new List<Container>();
            items.ForEach(i => instances.Add(SpawnItem(i, position)));
            instances.ForEach(i => spawnedContainers.Add(i));
            return instances;
        }
    }
}