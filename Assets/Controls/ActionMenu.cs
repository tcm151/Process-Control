using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using ProcessControl.Tools;
using ProcessControl.Industry;
#pragma warning disable 108,114


namespace ProcessControl.Controls
{
    public class ActionMenu : MonoBehaviour
    {
        private Camera camera;
        
        private void Awake()
        {
            camera = Camera.main;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                var colliders = Physics2D.OverlapCircleAll(camera.MousePosition2D(), 1f).ToList();

                var containers = new List<Container>();
                colliders.ForEach(c =>
                {
                    var container = c.GetComponent<Container>();
                    if (container is { }) containers.Add(container);
                });
                
                
            }
        }
    }
}