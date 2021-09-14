#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.Build.Reporting;


namespace ProcessControl.Tools.Editor
{
    public class BuildWindow : EditorWindow
    {
        private static string version;
        
        private static BuildWindow window;
        
        [MenuItem("Build/Build Window")]
        private static void ShowWindow()
        {
            window = GetWindow<BuildWindow>();
            window.titleContent = new GUIContent("Build Window");
            window.Show();

            
        }

        private void OnEnable()
        {
            version = PlayerSettings.bundleVersion;
        }

        private void OnGUI()
        {
            GUILayout.Label("Build Settings", EditorStyles.boldLabel);
            GUILayout.Space(4);
            version = EditorGUILayout.TextField("Version", version);
            GUILayout.Space(4);
            
            if (GUILayout.Button("Build Game"))
            {
                BuildGame();
            }
        }

        private void BuildGame()
        {
            var report = BuildPipeline.BuildPlayer(new BuildPlayerOptions
            {
                scenes = new [] {"Assets/Scenes/The Cave.unity"},
                locationPathName = $"Builds/{version}/Disjointed.exe",
                target = BuildTarget.StandaloneWindows64,
                options = BuildOptions.None,
            });

            if (report.summary.result == BuildResult.Succeeded)
            {
                Debug.Log($"Build succeeded: {report.summary.totalSize/1e6} MB");

                UpdateVersionNumber();
            }

            if (report.summary.result == BuildResult.Failed)
            {
                Debug.Log("Build failed!");
            }
        }

        private static void UpdateVersionNumber()
        {
            var number = version.Split('.');
            number[2] = (int.Parse(number[2]) + 1).ToString();
            version = $"{number[0]}.{number[1]}.{number[2]}";
            
            PlayerSettings.bundleVersion = version;
        }
    }
}
#endif