using UnityEngine;
using UnityEngine.UI;

namespace ProcessControl.UI
{
    [RequireComponent(typeof(Canvas))]
    [RequireComponent(typeof(CanvasGroup))]
    [RequireComponent(typeof(CanvasRenderer))]
    [RequireComponent(typeof(GraphicRaycaster))]
    abstract public class UI_Window : MonoBehaviour
    {
        public void Show()
        {
            //@ add tweening animation
            group.alpha = 1;
            group.interactable = true;
        }

        public void Hide()
        {
            //@ add tweening animation
            group.alpha = 0;
            group.interactable = false;
        }

        abstract public void GoBack();

        public float Height => transform.rect.height;
        public float Width => transform.rect.width;
        
        protected Canvas canvas;
        protected CanvasGroup group;
        protected GraphicRaycaster raycaster;
        new protected RectTransform transform;
        new protected CanvasRenderer renderer;
        
        virtual protected void Awake()
        {
            canvas    = GetComponent<Canvas>();
            group     = GetComponent<CanvasGroup>();
            renderer  = GetComponent<CanvasRenderer>();
            transform = GetComponent<RectTransform>();
            raycaster = GetComponent<GraphicRaycaster>();
        }
    }
}
