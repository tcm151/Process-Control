using UnityEngine;


namespace ProcessControl.UI
{

    [RequireComponent(typeof(CanvasGroup))]
    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(CanvasRenderer))]
    abstract public class UI_Panel : MonoBehaviour
    {
        protected CanvasGroup group;
        new protected RectTransform transform;
        new protected CanvasRenderer renderer;

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

        virtual protected void Awake()
        {
            group = GetComponent<CanvasGroup>();
            transform = GetComponent<RectTransform>();
            renderer = GetComponent<CanvasRenderer>();
        }
    }
}