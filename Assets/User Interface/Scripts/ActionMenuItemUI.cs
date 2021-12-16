using System;
using UnityEngine;
using UnityEngine.EventSystems;


namespace ProcessControl.User_Interface.Scripts
{
    public class ActionMenuItemUI : MonoBehaviour, IPointerDownHandler
    {
        public Action action;

        private void Awake()
        {
            action = () => Debug.Log("Button Pressed!");
        }

        public void OnPointerDown(PointerEventData data)
        {
            // Debug.Log("Clicked Action Item!");
            action.Invoke();
        }

    }
}