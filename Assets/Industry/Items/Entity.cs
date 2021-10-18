using System;
using UnityEngine;
using UnityEngine.Serialization;
#pragma warning disable 108,114


namespace ProcessControl.Industry.Resources
{
    //@ TODO convert this to a generic conveyor object
    public class Entity : MonoBehaviour
    {
        public static int Count = 0;
        
        public int ticks;
        
        internal Item item;

        private SpriteRenderer renderer;


        public Sprite sprite => item.sprite;
        
        public Vector3 position
        {
            get => transform.position;
            set => transform.position = value;
        }
        
        public void SetVisible(bool isVisible) => renderer.enabled = isVisible;
        public void ToggleVisible() => renderer.enabled = !renderer.enabled;

        public void SetProperties(Resource newResource)
        {
            item = newResource;
            renderer.sprite = item.sprite;
            gameObject.name = item.name;
        }
        
        private void Awake()
        {
            renderer = GetComponent<SpriteRenderer>();
            // renderer.sprite = resourceProperties.sprite;
        }

        public void OnDestroy() => Destroy(gameObject);
    }
}