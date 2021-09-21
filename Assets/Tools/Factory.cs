using UnityEngine;
using UnityEngine.SceneManagement;


namespace ProcessControl.Tools
{
    [CreateAssetMenu(fileName = "Factory", menuName = "Tools/Factory")]
    public class Factory : ScriptableObject
    {
        //> CREATE AND INSTANCE ON PREFAB
        public static T Spawn<T>(string sceneName, T prefab, Vector3 position) where T : MonoBehaviour
        {
            var scene = SceneManager.GetSceneByName(sceneName);
            if (!scene.isLoaded) scene = SceneManager.CreateScene(sceneName);

            T instance = Instantiate(prefab, position, Quaternion.identity);
            instance.name = prefab.name;
            SceneManager.MoveGameObjectToScene(instance.gameObject, scene);
            return instance;
        }

        public static T Spawn<T>(T prefab, Vector3 position, Quaternion rotation) where T : MonoBehaviour
            => Instantiate(prefab, position, rotation);

        public static T Spawn<T>(T prefab, Vector3 position) where T : MonoBehaviour
            => Spawn(prefab, position, Quaternion.identity);
        
        public static T Spawn<T>(T prefab) where T : MonoBehaviour
            => Spawn(prefab, Vector3.zero, Quaternion.identity);
    }
}