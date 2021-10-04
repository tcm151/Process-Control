using System;
using UnityEngine;
using UnityEngine.Serialization;


namespace ProcessControl.Industry.Resources
{
    public class Resource : MonoBehaviour
    {
        public static int Count = 0;
        
        public enum Material { Copper, Iron, Gold, Platinum, Coal, Stone, Sand, }
        public enum Type { Raw, Ingot, Plate, Gear, Wire, Cable, Screw, }
        
        public int ticks;
        
        [Serializable] public class Data
        {
            public string name;
            public Material material;
            public Type type;
            public Sprite sprite;
            
        }
        [FormerlySerializedAs("resource")][SerializeField] internal Data data;

        new private SpriteRenderer renderer;

        public Vector3 position
        {
            get => transform.position;
            set => transform.position = value;
        }
        
        public void SetVisible(bool isVisible) => renderer.enabled = isVisible;
        public void ToggleVisible() => renderer.enabled = !renderer.enabled;
        
        private void Awake()
        {
            renderer = GetComponent<SpriteRenderer>();
            renderer.sprite = data.sprite;
        }

        // private void FixedUpdate() => transform.position = resource.position;
        public void OnDestroy() => Destroy(gameObject);
    }
}