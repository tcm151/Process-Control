using System;
using System.Collections;
using System.Collections.Generic;
using ProcessControl.UI;
using UnityEngine;

namespace ProcessControl
{
    public class ControlHintsUI : UI_Panel
    {
        private bool showing;

        override protected void Awake()
        {
            base.Awake();
            showing = true;
            Show();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl))
            {
                showing = !showing;

                if (showing) Show();
                else Hide();
            }
        }
    }
}
