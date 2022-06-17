using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;


namespace ProcessControl.Tools
{
    abstract public class Factory : Service
    {
        abstract public T Get<T>(string name)
            where T : class;
        
        //> CREATE AND INSTANCE ON PREFAB
        public static T Spawn<T>(T prefab, Vector3 position, string sceneName = "Runtime")
            where T : Component
        {
            var scene = SceneManager.GetSceneByName(sceneName);
            if (!scene.isLoaded) scene = SceneManager.CreateScene(sceneName);

            var instance = Instantiate(prefab, position, Quaternion.identity);
            instance.name = prefab.name;
            SceneManager.MoveGameObjectToScene(instance.gameObject, scene);
            return instance;
        }

        public static T Spawn<T>(T prefab, Vector3 position, Quaternion rotation)
            where T : Component
            => Instantiate(prefab, position, rotation);

        // public static T Spawn<T>(T prefab, Vector3 position) where T : Object
        //     => Spawn(prefab, position, Quaternion.identity);
        
        public static T Spawn<T>(T prefab) where T : Component
            => Spawn(prefab, Vector3.zero, Quaternion.identity);
    }
}