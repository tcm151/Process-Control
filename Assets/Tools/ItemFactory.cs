using System.Collections.Generic;
using ProcessControl.Industry.Resources;
using UnityEngine;


namespace ProcessControl.Tools
{
    // [CreateAssetMenu(menuName = "Resources/ItemFactory")]
    class ItemFactory : Factory
    {
        public string sceneName = "Items";
        public Container container;
        public List<Item> items;

        private static ItemFactory instance;
        public static ItemFactory Instance => instance;
        
        private void Awake()
        {
            instance = this;
        }

        public Container SpawnItem(Item item, Vector3 position)
        {
            var instance = Spawn(container, position);
            instance.SetItem(item);
            return instance;
        }

        public List<Container> SpawnItems(List<Item> items, Vector3 position)
        {
            var instances = new List<Container>();
            items.ForEach(i =>
            {
                instances.Add(SpawnItem(i, position));
            });
            return instances;
        }
    }
}