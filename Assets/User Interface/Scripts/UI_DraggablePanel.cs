using UnityEngine;
using UnityEngine.EventSystems;


namespace ProcessControl.UI
{
    abstract public class UI_DraggablePanel : UI_Panel, IDragHandler, IPointerDownHandler
    {
        private Canvas parentCanvas;

        protected override void Awake()
        {
            base.Awake();
            
            parentCanvas = GetComponentInParent<Canvas>();
        }

        public void OnDrag(PointerEventData eventData) => transform.anchoredPosition += eventData.delta / parentCanvas.scaleFactor;
        public void OnPointerDown(PointerEventData eventData) => transform.SetAsLastSibling();
    }
}