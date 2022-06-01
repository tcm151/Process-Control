using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;


namespace ProcessControl.Tools
{
    public class ServiceManager : MonoBehaviour
    {
        public static ServiceManager Current {get; private set;}

        [SerializeReference] private readonly List<Service> services = new List<Service>();
        
        public static Action<Service> RegisterService;
        public static Action<Service> RemoveService;

        public T RequestService<T>()
        where T : Service
        {
            if (!services.Any(s => s is T))
            {
                Debug.Log($"Service '{typeof(T).Name}' does not exist.");
                return default;
            }
            return services.Find(s => s is T) as T;
        }

        private void Awake()
        {
            Current = this;

            RegisterService += (newService) =>
            {
                services.Add(newService);
                Debug.Log($"Service {newService} successfully registered.");
            };
            RemoveService += (oldService) => services.Remove(oldService);

            //- check for dupes
            var duplicates = FindObjectsOfType<ServiceManager>();
            if (duplicates.Length > 1)
            {
                Debug.Log("More than 1 ServiceManager present.");
            }
        }
    }
}