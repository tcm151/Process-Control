using System.Collections.Generic;
using System.Threading.Tasks;
using ProcessControl.Industry;
using UnityEngine;
using ProcessControl.Tools;
#pragma warning disable 108,114

public class BoxSelect : MonoBehaviour
{
    private Camera camera;
    private Vector2 firstPosition, secondPosition;
    
    private void Awake()
    {
        camera = Camera.main;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            firstPosition = camera.MousePosition2D();
        }

        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            secondPosition = camera.MousePosition2D();

            // var colliders = Physics2D.OverlapAreaAll(firstPosition, secondPosition);
            var transforms = BoxSelection<Transform>(firstPosition, secondPosition);
            var containers = BoxSelection<Container>(firstPosition, secondPosition);

            Debug.Log(transforms.Count);
            Debug.Log(containers.Count);
        }
    }

    public List<T> BoxSelection<T>(Vector2 firstCorner, Vector2 secondCorner)
    {
        var colliders = Physics2D.OverlapAreaAll(firstCorner, secondCorner);
        var matches = new List<T>();
        colliders.ForEach(
            c =>
            {
                T component;
                if ((component = c.GetComponent<T>()) is {}) matches.Add(component);
            });
        return matches;
    }
}