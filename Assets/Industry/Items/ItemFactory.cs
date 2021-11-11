using System.Collections.Generic;
using UnityEngine;
using ProcessControl.Industry;


namespace ProcessControl.Tools
{
    public class ItemFactory : Factory
    {
        public string sceneName = "Items";
        public Container container;

        private void Awake() => Instance = this;
        public static ItemFactory Instance {get; private set;}

        private readonly List<Container> spawnedContainers = new List<Container>();
        
        

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