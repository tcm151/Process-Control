using System;
using System.Collections;
using System.Collections.Generic;
using ProcessControl.Industry;
using ProcessControl.Tools;
using UnityEditor.Scripting;
using UnityEngine;

namespace ProcessControl.UI
{
    public class GodModeUI : UI_Window
    {
        [SerializeField] private ConstructionManager cm;
        private bool showing;
        
        private void Start()
        {
            var constructionManager = ServiceManager.Current.RequestService<ConstructionManager>();
            if (!constructionManager)
            {
                Debug.Log("CONSTRUCTION MANAGER! Nope.");
            }
            showing = constructionManager.godModEnabled;
            
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
