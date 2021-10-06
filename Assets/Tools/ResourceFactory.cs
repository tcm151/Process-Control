using System;
using System.Collections.Generic;
using System.Linq;
using ProcessControl.Industry.Resources;
using UnityEngine;


namespace ProcessControl.Tools
{
    [CreateAssetMenu(menuName = "Resources/Resource")]
    public class ResourceFactory : Factory
    {
        public string sceneName = "Resources";

        public List<Resource> resourcePrefabs;
        
        public static Func<Resource.Material, Resource.Type, Vector3, Resource> SpawnResource;

        override protected void Awake() => SpawnResource += OnSpawnResource;

        public Resource OnSpawnResource(Resource.Material material, Resource.Type type, Vector3 position)
        {
            Debug.Log("Spawning resource!");
            var prefab = resourcePrefabs.FirstOrDefault(o => o.data.material == material && o.data.type == type);
            if (prefab is null)
            {
                Debug.Log("RESOURCE DOES NOT EXIST!");
                return null;
            }
            
            var instance = Spawn(sceneName, prefab, position);
            return instance;
        }
    }
}