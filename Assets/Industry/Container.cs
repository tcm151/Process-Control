using System;
using ProcessControl.Graphs;
using UnityEngine;

#pragma warning disable 108,114


namespace ProcessControl.Industry
{
    public class Container : Entity
    {
        [Header("Container")]
        [SerializeField] internal Stack stack;
        
        public void SetVisible(bool isVisible) => renderer.enabled = isVisible;

        // private void OnValidate()
        // {
        //     if (stack is null) return;
        //     
        //     Awake();
        //     SetItem(stack);
        // }

        protected override void Awake()
        {
            base.Awake();
            var enabledColor = renderer.color;
            enabledColor.a = EnabledAlpha;
            renderer.color = enabledColor;
        }
        
        public void SetItem(Stack newStack)
        {
            stack = newStack;
            renderer.sprite = stack.item.sprite;
            gameObject.name = stack.item.name;
        }
    }
}