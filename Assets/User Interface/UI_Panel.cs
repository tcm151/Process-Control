// using UnityEngine;
// using UnityEngine.UI;
//
// #pragma warning disable 108,114
//
//
// namespace ProcessControl.UI
// {
//
//     [RequireComponent(typeof(CanvasGroup))]
//     [RequireComponent(typeof(RectTransform))]
//     [RequireComponent(typeof(CanvasRenderer))]
//     [RequireComponent(typeof(GraphicRaycaster))]
//     abstract public class UI_Panel : MonoBehaviour
//     {
//         protected CanvasGroup group;
//         protected RectTransform transform;
//         protected CanvasRenderer renderer;
//
//         public void Show()
//         {
//             //@ add tweening animation
//             group.alpha = 1;
//             group.interactable = true;
//         }
//
//         public void Hide()
//         {
//             //@ add tweening animation
//             group.alpha = 0;
//             group.interactable = false;
//         }
//
//         virtual protected void Awake()
//         {
//             group = GetComponent<CanvasGroup>();
//             transform = GetComponent<RectTransform>();
//             renderer = GetComponent<CanvasRenderer>();
//         }
//     }
// }