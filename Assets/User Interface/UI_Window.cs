using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
#pragma warning disable 108,114

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
            raycaster.enabled = true;
        }

        public void Hide()
        {
            //@ add tweening animation
            group.alpha = 0;
            group.interactable = false;
            raycaster.enabled = false;
        }

        abstract public void GoBack();

        public float Height => transform.rect.height;
        public float Width => transform.rect.width;

        protected Canvas canvas;
        protected CanvasGroup group;
        protected GraphicRaycaster raycaster;
        protected CanvasRenderer renderer;
        new protected RectTransform transform;
        
        virtual protected void Awake()
        {
            canvas    = GetComponent<Canvas>();
            group     = GetComponent<CanvasGroup>();
            renderer  = GetComponent<CanvasRenderer>();
            transform = GetComponent<RectTransform>();
            raycaster = GetComponent<GraphicRaycaster>();
        }

    }

    abstract public class UI_DraggableWindow : UI_Window, IDragHandler, IPointerDownHandler
    {
        public void OnDrag(PointerEventData eventData) => transform.anchoredPosition += eventData.delta / canvas.scaleFactor;
        public void OnPointerDown(PointerEventData eventData) => transform.SetAsLastSibling();
    }
}
