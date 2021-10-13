

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
        private bool autoUpdateChunks;
        private bool autoUpdateOre;

        private void OnEnable()
        {
            grid = target as ProceduralGrid;
        }

        override public void OnInspectorGUI()
        {
            using var check = new EditorGUI.ChangeCheckScope();

            base.OnInspectorGUI();

            autoUpdateChunks = EditorGUILayout.Toggle("Auto Update Chunks", autoUpdateChunks);
            autoUpdateOre = EditorGUILayout.Toggle("Auto Update Ore", autoUpdateOre);
            if (check.changed)
            {
                if (autoUpdateChunks) grid.GenerateAllChunks();
                if (autoUpdateOre) grid.GenerateAllResources();
            }

            if (GUILayout.Button("Initialize & Generate"))
            {
                grid.Awake();
            }

            if (GUILayout.Button("Clear Grid")) grid.ClearAllTiles();
        }
    }
}

