using System;
using ProcessControl.Industry;
using UnityEngine;
#pragma warning disable 108,114


namespace ProcessControl.Graphs
{
    public class Entity : MonoBehaviour
    {
        internal const int TicksPerSecond = 64;
        internal static int TicksPerMinute => TicksPerSecond * 60;

        [Header("Entity")]
        new public bool enabled;
        public bool sleeping;
        public int ticks;
        public int sleepThreshold = 2048;

        protected const int EnabledAlpha = 255;
        protected const int DisabledAlpha = 100;

        internal SpriteRenderer renderer;
        public Sprite sprite => renderer.sprite;

        [Header("Schematic")]
        public Schematic schematic;
        
        public Vector3 position
        {
            get => transform.position;
            set => transform.position = value;
        }

        virtual protected void Awake()
        {
            renderer = GetComponent<SpriteRenderer>();
            if (schematic is { })
            {
                renderer.sprite = schematic.sprite;
            }
            
            var disabledColor = renderer.color;
            disabledColor.a = DisabledAlpha / 255f;
            renderer.color = disabledColor;
        }

        virtual protected void OnDestroy() => Destroy(gameObject);
    }
}