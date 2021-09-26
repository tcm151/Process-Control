

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
            if (check.changed && autoUpdate)
            {
                grid.GenerateAllChunks();
                grid.GenerateAllResources();
            }

            if (GUILayout.Button("Initialize & Generate"))
            {
                grid.Initialize();
                grid.GenerateAllChunks();
                grid.GenerateAllResources();
            }

            if (GUILayout.Button("Clear Grid")) grid.ClearTiles();
        }
    }
}

