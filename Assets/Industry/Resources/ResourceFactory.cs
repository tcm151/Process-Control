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

        //> PUBLIC EVENT 
        public static Func<Resource.Material, Resource.Type, Vector3, Resource> SpawnResource;
        override protected void OnBegin() => SpawnResource += OnSpawnResource;

        //> SPAWN A RESOURCE OF MATCHING MATERIAL AND TYPE
        private Resource OnSpawnResource(Resource.Material material, Resource.Type type, Vector3 position)
        {
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