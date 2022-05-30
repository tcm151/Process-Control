using System;
using System.Collections;
using System.Collections.Generic;
using ProcessControl.Industry;
using UnityEngine;

namespace ProcessControl.UI
{
    public class GodModeUI : UI_Window
    {
        [SerializeField] private ConstructionManager cm;
        private bool showing;
        
        protected override void Awake()
        {
            base.Awake();
            showing = !cm.queueJobGlobal;
            
            if (showing) Show();
            else Hide();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                showing = !showing;
                
                if (showing) Show();
                else Hide();
            }
        }
    }
}
