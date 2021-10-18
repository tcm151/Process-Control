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
        
        internal ResourceProperties resourceProperties;

        private SpriteRenderer renderer;


        public Sprite sprite => resourceProperties.sprite;
        
        public Vector3 position
        {
            get => transform.position;
            set => transform.position = value;
        }
        
        public void SetVisible(bool isVisible) => renderer.enabled = isVisible;
        public void ToggleVisible() => renderer.enabled = !renderer.enabled;

        public void SetProperties(ResourceProperties newProperties)
        {
            resourceProperties = newProperties;
            renderer.sprite = resourceProperties.sprite;
            gameObject.name = resourceProperties.name;
        }
        
        private void Awake()
        {
            renderer = GetComponent<SpriteRenderer>();
            // renderer.sprite = resourceProperties.sprite;
        }

        public void OnDestroy() => Destroy(gameObject);
    }
}