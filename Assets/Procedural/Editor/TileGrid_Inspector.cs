

using UnityEngine;
using UnityEditor;
using ProcessControl.Procedural;


namespace ProcessControl.Tools.Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(CellGrid))]
    public class TileGrid_Inspector : UnityEditor.Editor
    {
        private CellGrid grid;
        private bool autoUpdateChunks;
        private bool autoUpdateTerrain;
        private bool autoUpdateResources;

        private void OnEnable()
        {
            grid = target as CellGrid;
        }

        override public void OnInspectorGUI()
        {
            using var check = new EditorGUI.ChangeCheckScope();

            base.OnInspectorGUI();

            autoUpdateChunks = EditorGUILayout.Toggle("Auto Update Chunks", autoUpdateChunks);
            autoUpdateTerrain = EditorGUILayout.Toggle("Auto Update Biomes", autoUpdateTerrain);
            autoUpdateResources = EditorGUILayout.Toggle("Auto Update Ore", autoUpdateResources);
            if (check.changed)
            {
                if (autoUpdateChunks) grid.GenerateAllChunks();
                if (autoUpdateTerrain) grid.GenerateAllTerrain();
                if (autoUpdateResources) grid.GenerateAllResources();
            }

            if (GUILayout.Button("Generate Grid"))
            {
                grid.Awake();
            }

            if (GUILayout.Button("Clear Grid")) grid.ClearAllTiles();
        }
    }
}

