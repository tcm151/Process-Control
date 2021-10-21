using System.Collections.Generic;
using ProcessControl.Industry.Resources;
using UnityEngine;


namespace ProcessControl.Tools
{
    public class ItemFactory : Factory
    {
        public string sceneName = "Items";
        public Container container;

        private void Awake() => Instance = this;
        public static ItemFactory Instance {get; private set;}

        public Container SpawnItem(Item item, Vector3 position)
        {
            var instance = Spawn(sceneName, container, position);
            instance.SetItem(item);
            return instance;
        }

        public List<Container> SpawnItems(List<Item> items, Vector3 position)
        {
            var instances = new List<Container>();
            items.ForEach(i => instances.Add(SpawnItem(i, position)));
            return instances;
        }
    }
}