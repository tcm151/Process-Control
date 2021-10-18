using System.Collections.Generic;
using ProcessControl.Industry.Resources;
using UnityEngine;


namespace ProcessControl.Tools
{
    [CreateAssetMenu(menuName = "Resources/ItemFactory")]
    class ItemFactory : Factory
    {
        public string sceneName = "Items";
        public Entity container;
        public List<Item> items;

        private static ItemFactory instance;
        public static ItemFactory Instance => instance;
        
        override protected void OnBegin()
        {
            instance = this;
        }

        public Entity SpawnItem(Item item, Vector3 position)
        {
            var instance = Spawn(container, position);
            instance.SetItem(item);
            return instance;
        }

        public List<Entity> SpawnItems(List<Item> items, Vector3 position)
        {
            var instances = new List<Entity>();
            items.ForEach(i =>
            {
                instances.Add(SpawnItem(i, position));
            });
            return instances;
        }
    }
}