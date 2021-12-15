using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;


namespace ProcessControl.User_Interface.Scripts
{
    public class ActionMenuItemUI : MonoBehaviour, IPointerClickHandler
    {
        public Action action;

        private void Awake()
        {
            action = () => Debug.Log("Button Pressed!");
        }

        public void OnPointerClick(PointerEventData data)
        {
            Debug.Log("Clicked Action Item!");
            action.Invoke();
        }
    }
}