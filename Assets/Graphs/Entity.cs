using UnityEngine;
#pragma warning disable 108,114


namespace ProcessControl.Graphs
{
    public class Entity : MonoBehaviour
    {
        public Color enabledColor = new Color(255, 255, 255, 255);
        public Color disabledColor = new Color(255, 255, 255, 100);


        public Sprite sprite => renderer.sprite;
        internal SpriteRenderer renderer;

        virtual protected void Awake()
        {
            renderer = GetComponent<SpriteRenderer>();
            renderer.color = disabledColor;
        }
    }
}