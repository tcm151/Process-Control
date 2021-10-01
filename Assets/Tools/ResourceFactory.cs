using System.Collections.Generic;
using System.Linq;
using ProcessControl.Industry.Resources;
using UnityEngine;


namespace ProcessControl.Tools
{
    public class ResourceFactory : Factory
    {
        public List<Resource> resourcePrefabs;

        public Resource SpawnResource(Resource.Material material, Resource.Type type, Vector3 position)
        {
            var prefab = resourcePrefabs.FirstOrDefault(o => o.resource.material == material && o.resource.type == type);
            if (prefab is null)
            {
                Debug.Log("RESOURCE DOES NOT EXIST!");
                return null;
            }
            
            var instance = Spawn("Resources", prefab, position);
            return instance;
        }
    }
}