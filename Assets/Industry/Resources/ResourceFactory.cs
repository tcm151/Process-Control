﻿using System;
using System.Collections.Generic;
using System.Linq;
using ProcessControl.Industry.Resources;
using UnityEngine;
using UnityEngine.Serialization;


namespace ProcessControl.Tools
{
    [CreateAssetMenu(menuName = "Resources/Factory")]
    public class ResourceFactory : Factory
    {
        public string sceneName = "Resources";
        public Resource prefab;
        [FormerlySerializedAs("resourcePrefabs")] public List<ResourceProperties> resources;

        //> PUBLIC EVENT 
        public static Func<ResourceProperties.Material, ResourceProperties.Form, Vector3, Resource> SpawnResource;
        public void OnEnable() => SpawnResource += OnSpawnResource;
        
        //> SPAWN A RESOURCE OF MATCHING MATERIAL AND TYPE
        private Resource OnSpawnResource(ResourceProperties.Material material, ResourceProperties.Form type, Vector3 position)
        {
            var properties = resources.FirstOrDefault(o => o.material == material && o.form == type);
            if (properties is null)
            {
                Debug.Log("RESOURCE DOES NOT EXIST!");
                return null;
            }
            
            var instance = Spawn(sceneName, prefab, position);
            instance.SetProperties(properties);
            return instance;
        }
    }
}