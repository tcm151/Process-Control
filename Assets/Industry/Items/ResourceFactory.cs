// using System;
// using System.Collections.Generic;
// using System.Linq;
// using ProcessControl.Industry.Resources;
// using UnityEngine;
// using UnityEngine.Serialization;
//
//
// namespace ProcessControl.Tools
// {
//     // [CreateAssetMenu(menuName = "Resources/Factory")]
//     public class ResourceFactory : Factory
//     {
//         public string sceneName = "Resources";
//         public Container prefab;
//         public List<Resource> resources;
//
//         //> PUBLIC EVENT 
//         public static Func<Resource.Material, Resource.Form, Vector3, Container> SpawnResource;
//         // private void Aw() => SpawnResource += OnSpawnResource;
//
//         private void Awake()
//         {
//             // SpawnResource += OnSpawnResource;
//         }
//
//         //> SPAWN A RESOURCE OF MATCHING MATERIAL AND TYPE
//         private Container OnSpawnResource(Resource.Material material, Resource.Form type, Vector3 position)
//         {
//             var properties = resources.FirstOrDefault(o => o.material == material);
//             if (properties is null)
//             {
//                 Debug.Log("RESOURCE DOES NOT EXIST!");
//                 return null;
//             }
//             
//             var instance = Spawn(sceneName, prefab, position);
//             instance.SetItem(properties);
//             return instance;
//         }
//     }
// }