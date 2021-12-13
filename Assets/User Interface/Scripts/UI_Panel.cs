using System;
using System.Threading.Tasks;
using ProcessControl.Tools;
using UnityEngine;
#pragma warning disable 108,114


namespace ProcessControl.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    [RequireComponent(typeof(RectTransform))]
    public class UI_Panel : MonoBehaviour
    {
        [SerializeField, HideInInspector] protected CanvasGroup group;
        [SerializeField, HideInInspector] protected RectTransform transform;
        
        public async void Show(float duration = 0.33f)
        {
            group.blocksRaycasts = true;
            group.interactable = true;
            
            await Alerp.Interval(duration, (elapsedTime) =>
            {
                group.alpha = Mathf.Lerp(group.alpha, 1, elapsedTime / duration);
            });

            group.alpha = 1;
        }

        public async void Hide(float duration = 0.33f)
        {
            group.blocksRaycasts = false;
            group.interactable = false;
            
            await Alerp.Interval(duration,(elapsedTime) =>
            {
                group.alpha = Mathf.Lerp(group.alpha, 0, elapsedTime / duration);
            });

            group.alpha = 0;
        }

        virtual protected void Awake()
        {
            group = GetComponent<CanvasGroup>();
            transform = GetComponent<RectTransform>();
        }
    }
}