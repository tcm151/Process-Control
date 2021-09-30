using System;
using UnityEngine;


namespace ProcessControl.Industry.Resources
{
    public class Resource : MonoBehaviour
    {
        public enum Type { Copper, Iron, Gold, Platinum }
        
        [Serializable] public class Data
        {
            public int ticks;
            public bool sleeping;
            public int sleepThreshold = 512;

            public Sprite sprite;
            
            public Type type;
            
            public Vector3 position;
        }
        [SerializeField] internal Data data;

        new private SpriteRenderer renderer;

        public void SetSprite(Sprite newSprite) => renderer.sprite = newSprite;
        public void SetVisible(bool visible) => renderer.enabled = visible;

        public Vector3 position => data.position;

        private void Awake()
        {
            data.position = transform.position;
            renderer = GetComponent<SpriteRenderer>();
        }

        private void FixedUpdate()
        {
            transform.position = data.position;
        }

        public void OnDestroy() => Destroy(gameObject);
    }
}