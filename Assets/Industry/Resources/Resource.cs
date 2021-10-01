using System;
using UnityEngine;
using UnityEngine.Serialization;


namespace ProcessControl.Industry.Resources
{
    public class Resource : MonoBehaviour
    {
        public enum Material { Copper, Iron, Gold, Platinum, Stone, Sand, }
        public enum Type { Ore, Ingot, Plate, Gear, Wire, Cable, Screw, }
        
        [Serializable] public class Data
        {
            public int ticks;

            [FormerlySerializedAs("type")] public Material material;
            public Type type;
            public Sprite sprite;
            
            public Vector3 position;
        }
        [FormerlySerializedAs("data")][SerializeField] internal Data resource;

        new private SpriteRenderer renderer;

        public Vector3 position => resource.position;

        public void SetSprite(Sprite newSprite) => renderer.sprite = newSprite;
        public void SetVisible(bool visible) => renderer.enabled = visible;

        private void Awake()
        {
            resource.position = transform.position;
            renderer = GetComponent<SpriteRenderer>();
        }

        private void FixedUpdate() => transform.position = resource.position;
        public void OnDestroy() => Destroy(gameObject);
    }
}