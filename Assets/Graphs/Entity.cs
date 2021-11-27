using System;
using UnityEngine;
#pragma warning disable 108,114


namespace ProcessControl.Graphs
{
    public class Entity : MonoBehaviour
    {
        protected const int TicksPerSecond = 64;
        protected static int TicksPerMinute => TicksPerSecond * 60;

        new public bool enabled;
        public int enabledAlpha = 255;
        public int disabledAlpha = 100;
        // public Color enabledColor = new Color(255, 255, 255, 255);
        // public Color disabledColor = new Color(255, 255, 255, 100);

        internal SpriteRenderer renderer;
        public Sprite sprite => renderer.sprite;
        
        public Vector3 position
        {
            get => transform.position;
            set => transform.position = value;
        }

        virtual protected void Awake()
        {
            renderer = GetComponent<SpriteRenderer>();
            var disabledColor = renderer.color;
            disabledColor.a = disabledAlpha / 255f;
            renderer.color = disabledColor;
        }

        virtual protected void OnDestroy() => Destroy(gameObject);
    }
}