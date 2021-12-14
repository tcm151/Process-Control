using System;
using ProcessControl.Tools;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
#pragma warning disable 108,114


namespace ProcessControl.UI
{
    // [ExecuteInEditMode]
    [RequireComponent(typeof(LayoutElement))]
    public class TooltipPanel : UI_Panel
    {
        private LayoutElement layout;
        
        public static Action<string, string> ShowTooltip;
        public static Action<float> HideTooltip;

        private Camera camera;
        
        override protected void Awake()
        {
            base.Awake();
            ShowTooltip += OnShow;
            HideTooltip += Hide;

            camera = Camera.main;
            layout = GetComponent<LayoutElement>();
        }

        public void OnShow(string headerText = null, string contentText = null)
        {
            header.gameObject.SetActive(headerText is { });
            header.text = headerText;
            
            content.gameObject.SetActive(contentText is { });
            content.text = contentText;

            if (header.text is { } && content.text is { })
            {
                int headerLength = header.text.Length;
                int contentLength = content.text.Length;

                layout.enabled = (headerLength > wrapSize || contentLength > wrapSize);
            }
            
            Show();
        }
        
        
        public TextMeshProUGUI header;
        public TextMeshProUGUI content;

        public int wrapSize;

        private void Update()
        {
            if (Application.isEditor)
            {
                int headerLength = header.text.Length;
                int contentLength = content.text.Length;

                layout.enabled = (headerLength > wrapSize || contentLength > wrapSize);
            }

            transform.pivot = new Vector2(Input.mousePosition.x / Screen.width, Input.mousePosition.y / Screen.height);
            transform.position = camera.MousePosition2D();
        }
    }
}