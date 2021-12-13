using UnityEngine;
using UnityEngine.EventSystems;


namespace ProcessControl.UI
{
    [RequireComponent(typeof(Canvas))]
    abstract public class UI_DraggableWindow : UI_Window, IDragHandler, IPointerDownHandler
    {
        private Canvas canvas;

        override protected void Awake()
        {
            base.Awake();
            canvas = GetComponentInParent<Canvas>();
        }

        public void OnDrag(PointerEventData eventData) => transform.anchoredPosition += eventData.delta / canvas.scaleFactor;
        public void OnPointerDown(PointerEventData eventData) => transform.SetAsLastSibling();
    }
}