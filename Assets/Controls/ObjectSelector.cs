using System;
using System.Collections.Generic;
using UnityEngine;
using ProcessControl.Tools;
using ProcessControl.Industry;
using UnityEngine.EventSystems;

#pragma warning disable 108,114

public class ObjectSelector : MonoBehaviour
{
    private Camera camera;
    private Vector2 firstPosition, secondPosition;

    public static event Action<Vector3> onOpenActionMenu;
    public static event Action onCloseActionMenu;

    private bool buildMode;
    private bool overUI;
    
    private void Awake()
    {
        camera = Camera.main;

        ConstructionManager.OnBuildModeChanged += (truth) => buildMode = truth;
    }

    private void Update()
    {
        if (buildMode) return;
        
        if (Input.GetMouseButtonDown(1))
        {
            onOpenActionMenu?.Invoke(camera.MousePosition2D());
        }
        
        if (Input.GetMouseButtonDown(0))
        {
            Alerp.DelayedAction(0.1f, () => onCloseActionMenu?.Invoke());
            
            firstPosition = camera.MousePosition2D();
            overUI = EventSystem.current.IsPointerOverGameObject();
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (overUI) return;

            secondPosition = camera.MousePosition2D();
            var transforms = BoxSelection<Transform>(firstPosition, secondPosition);
            Debug.Log(transforms.Count);
        }
    }

    private static List<T> BoxSelection<T>(Vector2 firstCorner, Vector2 secondCorner)
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