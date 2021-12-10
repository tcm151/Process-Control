using System;
using ProcessControl.Graphs;
using UnityEngine;

#pragma warning disable 108,114


namespace ProcessControl.Industry
{
    public class Container : Entity
    {
        [Header("Container")]
        [SerializeField] internal Item item;
        // [SerializeField] internal int ticks;
        
        public void SetVisible(bool isVisible) => renderer.enabled = isVisible;

        private void OnValidate()
        {
            if (item is { })
            {
                Awake();
                SetItem(item);
            }
        }

        public void SetItem(Item newItem)
        {
            item = newItem;
            renderer.sprite = item.sprite;
            gameObject.name = item.name;
        }
        
        override protected void Awake()
        {
            renderer = GetComponent<SpriteRenderer>();
            var enabledColor = renderer.color;
            enabledColor.a = EnabledAlpha;
            renderer.color = enabledColor;
        }
    }
}