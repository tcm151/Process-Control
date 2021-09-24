

using UnityEngine;
using UnityEditor;
using ProcessControl.Procedural;


namespace ProcessControl.Tools.Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(ProceduralGrid))]
    public class ProceduralGrid_Inspector : UnityEditor.Editor
    {
        private ProceduralGrid grid;
        private bool autoUpdate;
        
        private void OnEnable()
        {
            grid = target as ProceduralGrid;
        }

        override public void OnInspectorGUI()
        {
            using var check = new EditorGUI.ChangeCheckScope();

            base.OnInspectorGUI();
            
            autoUpdate = EditorGUILayout.Toggle("Auto Update", autoUpdate);
            if (check.changed && autoUpdate) grid.GenerateOre();

            if (GUILayout.Button("Generate"))
            {
                grid.GenerateOre();
            }
        }
    }
}

