using System;
using UnityEditor;
using UnityEngine;


namespace ProcessControl.UI
{
    class PauseWindow : UI_Window
    {
        override public void GoBack() => Resume();

        public static bool IsPaused => Mathf.Approximately(Time.timeScale, 0f);

        override protected void Awake()
        {
            base.Awake();
            Hide();
        }
        
        private void Update()
        {
            #if UNITY_EDITOR
                if (Input.GetKeyDown(KeyCode.Backspace))
            #else
                if (Input.GetKeyDown(KeyCode.Escape))
            #endif
            {
                ToggleGameState();
            }
        }

        public void Pause()
        {
            Time.timeScale = 0;
            Show();
        }

        public void Resume()
        {
            Hide();
            Time.timeScale = 1;
        }

        public void ToggleGameState()
        {
            if (IsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }

        public void ExitToMenu() { }
        public void QuitApplication()
        {
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #else
            Application.Quit();
            #endif
        }
    }
}