using UnityEngine;

#pragma warning disable 108,114


namespace ProcessControl.Industry.Resources
{
    //@ TODO convert this to a generic conveyor object
    public class Container : MonoBehaviour
    {
        [SerializeField] internal Item item;
        
        internal int ticks;

        private SpriteRenderer renderer;
        public Sprite sprite => item.sprite;

        public Vector3 position
        {
            get => transform.position;
            set => transform.position = value;
        }
        
        public void SetVisible(bool isVisible) => renderer.enabled = isVisible;

        public void SetItem(Item newItem)
        {
            item = newItem;
            renderer.sprite = item.sprite;
            gameObject.name = item.name;
        }
        
        private void Awake()
        {
            renderer = GetComponent<SpriteRenderer>();
            renderer.sprite = item.sprite;
        }

        public void OnDestroy() => Destroy(gameObject);
    }
}