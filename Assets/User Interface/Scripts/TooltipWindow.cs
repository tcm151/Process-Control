using System;
using UnityEngine;
using TMPro;


namespace ProcessControl.UI
{
    // [ExecuteInEditMode]
    public class TooltipWindow : UI_Window
    {
        public static Action<string, string> ShowTooltip;
        public static Action HideTooltip;

        override protected void Awake()
        {
            base.Awake();
            ShowTooltip += OnShow;
            HideTooltip += Hide;
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
            // raycaster.enabled = false;
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
            transform.position = Input.mousePosition;
        }
    }
}