using System;
using ProcessControl.Tools;
using UnityEngine;

namespace ProcessControl.Building
{
    public class BuildManager : MonoBehaviour
    {
        public MonoBehaviour currentBuildItem;

        private bool buildMode;
        private Camera camera;
        
        public static Action<bool> OnBuildModeChanged;
        public static Action<MonoBehaviour> SetBuildItem;

        private void Awake()
        {
            camera = Camera.main;

            SetBuildItem += OnSetBuildItem;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.B))
            {
                buildMode = !buildMode;
                OnBuildModeChanged?.Invoke(buildMode);
            }
            
            if (!buildMode || !currentBuildItem) return;

            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                var mousePosition = camera.ViewportToWorldPoint(Input.mousePosition);
                mousePosition.z = 0f;
                
                Factory.Spawn(currentBuildItem, mousePosition);
            }
        }

        private void OnSetBuildItem(MonoBehaviour newBuildItem)
        {
            currentBuildItem = newBuildItem;
        }
    }
}
