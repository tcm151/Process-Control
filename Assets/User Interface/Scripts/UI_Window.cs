using System;
using UnityEngine;
using UnityEngine.UI;

#pragma warning disable 108,114

namespace ProcessControl.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    [RequireComponent(typeof(CanvasRenderer))]
    [RequireComponent(typeof(GraphicRaycaster))]
    abstract public class UI_Window : MonoBehaviour
    {
        public void Show()
        {
            if (!initialized) Initialize();
            
            //@ add tweening animation
            group.alpha = 1;
            group.interactable = true;
            raycaster.enabled = true;
        }

        public void Hide()
        {
            if (!initialized) Initialize();
            
            //@ add tweening animation
            group.alpha = 0;
            group.interactable = false;
            raycaster.enabled = false;
        }

        private void OnValidate()
        {
            if (!initialized) Initialize();
        }

        virtual public void GoBack() => Hide();

        public float Width => transform.rect.width;
        public float Height => transform.rect.height;

        protected CanvasGroup group;
        protected LayoutElement layout;
        protected CanvasRenderer renderer;
        protected GraphicRaycaster raycaster;
        new protected RectTransform transform;

        private bool initialized;
        
        virtual protected void Awake() => Initialize();

        private void Initialize()
        {
            group     ??= GetComponent<CanvasGroup>();
            layout    ??= GetComponent<LayoutElement>();
            renderer  ??= GetComponent<CanvasRenderer>();
            transform ??= GetComponent<RectTransform>();
            raycaster ??= GetComponent<GraphicRaycaster>();
            initialized = true;
        }
    }
}
