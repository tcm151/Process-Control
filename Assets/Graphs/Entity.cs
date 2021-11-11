using UnityEngine;
#pragma warning disable 108,114


namespace ProcessControl.Graphs
{
    public class Entity : MonoBehaviour
    {
        protected const int TicksPerSecond = 64;
        protected static int TicksPerMinute => TicksPerSecond * 60;
        
        public Color enabledColor = new Color(255, 255, 255, 255);
        public Color disabledColor = new Color(255, 255, 255, 100);

        internal SpriteRenderer renderer;
        public Sprite sprite => renderer.sprite;

        virtual protected void Awake()
        {
            renderer = GetComponent<SpriteRenderer>();
            renderer.color = disabledColor;
        }
    }
}