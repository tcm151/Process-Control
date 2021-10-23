using UnityEngine;
using UnityEditor;


namespace ProcessControl.UI
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(UI_Window), true)]
    public class UI_Window_Inspector : Editor
    {
        private UI_Window window;
        
        //> ON ENABLE
        private void OnEnable() => window = target as UI_Window;

        //> INSPECTOR GUI
        override public void OnInspectorGUI()
        {
            // EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Show")) window.Show();
            if (GUILayout.Button("Hide")) window.Hide();
            // EditorGUILayout.EndHorizontal();
            
            base.OnInspectorGUI();
        }
    }
}