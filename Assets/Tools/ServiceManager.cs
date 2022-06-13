using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace ProcessControl.Tools
{
	public class ServiceManager : MonoBehaviour
	{
		public static ServiceManager Current { get; private set; }

		[SerializeReference] private readonly List<Service> services = new List<Service>();

		public static Action<Service> RegisterService;
		public static Action<Service> RemoveService;

		public T RequestService<T>() where T : Service
		{
			if (services.Any(s => s is T)) return services.Find(s => s is T) as T;
			
			Debug.Log($"Service '{typeof(T)}' did not exist, creating new instance...");
			return CreateService<T>();
			// return default;
		}

		public T CreateService<T>() where T : Service
		{
			Debug.Log($"Creating service '{typeof(T).Name}'");
			var service = new GameObject($"{nameof(T)}").AddComponent<T>();
			service.transform.SetParent(transform);
			service.Initialize();
			// RegisterService(service);
			return service;
		}

		public static ServiceManager Create()
		{
			return new GameObject("ServiceManager").AddComponent<ServiceManager>();
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