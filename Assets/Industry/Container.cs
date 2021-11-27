using ProcessControl.Graphs;
using UnityEngine;

#pragma warning disable 108,114


namespace ProcessControl.Industry
{
    public class Container : Entity
    {
        [SerializeField] internal Item item;
        [SerializeField] internal int ticks;
        
        public void SetVisible(bool isVisible) => renderer.enabled = isVisible;

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
            enabledColor.a = enabledAlpha;
            renderer.color = enabledColor;
        }
    }
}