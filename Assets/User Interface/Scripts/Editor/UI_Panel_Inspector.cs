using UnityEngine;
using UnityEditor;


namespace ProcessControl.UI
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(UI_Panel), true)]
    public class UI_Panel_Inspector : Editor
    {
        private UI_Panel window;
        
        //> ON ENABLE
        private void OnEnable() => window = target as UI_Panel;

        //> INSPECTOR GUI
        public override void OnInspectorGUI()
        {
            // EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Show")) window.Show();
            if (GUILayout.Button("Hide")) window.Hide();
            // EditorGUILayout.EndHorizontal();
            
            base.OnInspectorGUI();
        }
    }
}