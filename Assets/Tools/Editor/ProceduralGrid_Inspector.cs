

using UnityEngine;
using UnityEditor;
using ProcessControl.Procedural;


namespace ProcessControl.Tools.Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(TileGrid))]
    public class ProceduralGrid_Inspector : UnityEditor.Editor
    {
        private TileGrid grid;
        private bool autoUpdateChunks;
        private bool autoUpdateBiomes;
        private bool autoUpdateOre;

        private void OnEnable()
        {
            grid = target as TileGrid;
        }

        override public void OnInspectorGUI()
        {
            using var check = new EditorGUI.ChangeCheckScope();

            base.OnInspectorGUI();

            autoUpdateChunks = EditorGUILayout.Toggle("Auto Update Chunks", autoUpdateChunks);
            autoUpdateBiomes = EditorGUILayout.Toggle("Auto Update Biomes", autoUpdateBiomes);
            autoUpdateOre = EditorGUILayout.Toggle("Auto Update Ore", autoUpdateOre);
            if (check.changed)
            {
                if (autoUpdateChunks) grid.GenerateAllChunks();
                if (autoUpdateBiomes) grid.GenerateAllBiomes();
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

