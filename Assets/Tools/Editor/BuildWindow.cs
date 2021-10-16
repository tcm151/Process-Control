#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine.SceneManagement;


namespace ProcessControl.Tools.Editor
{
    public class BuildWindow : EditorWindow
    {
        private static string version;
        // private static string sceneNames;
        private static BuildWindow window;
        
        [MenuItem("Build/Build Window")]
        private static void ShowWindow()
        {
            window = GetWindow<BuildWindow>();
            window.titleContent = new GUIContent("Build Window");
            window.Show();
        }

        private void OnEnable() => version = PlayerSettings.bundleVersion;

        private void OnGUI()
        {
            
            GUILayout.Label("Build Settings", EditorStyles.boldLabel);
            GUILayout.Space(4);
            version = EditorGUILayout.TextField("Version", version);
            // EditorGUILayout.LabelField("Scenes In Build");
            // sceneNames = EditorGUILayout.TextArea(sceneNames);
            GUILayout.Space(4);
            
            if (GUILayout.Button("Dev Build")) BuildGame(0);
            if (GUILayout.Button("Beta Build")) BuildGame(1);
            if (GUILayout.Button("Release Build")) BuildGame(2);

        }

        private void BuildGame(int buildType)
        {
            var buildReport = BuildPipeline.BuildPlayer(new BuildPlayerOptions
            {
                scenes = new [] {$"Assets/Terrain Generation.unity"},
                locationPathName = $"Builds/{version}/{PlayerSettings.productName}.exe",
                target = BuildTarget.StandaloneWindows64,
                options = BuildOptions.None,
            });

            if (buildReport.summary.result == BuildResult.Succeeded)
            {
                Debug.Log($"Build succeeded: {buildReport.summary.totalSize/1e6} MB");
                UpdateVersionNumber(buildType);
            }

            if (buildReport.summary.result == BuildResult.Failed)
            {
                Debug.Log("Build failed!");
            }
        }

        private void UpdateVersionNumber(int buildType)
        {
            var number = version.Split('.');
            switch (buildType)
            {
                case 0:
                {
                    number[2] = (int.Parse(number[2]) + 1).ToString();
                    break;
                }
                
                case 1:
                {
                    number[1] = (int.Parse(number[1]) + 1).ToString();
                    number[2] = "0";
                    break;
                }
                
                case 2:
                {
                    number[0] = (int.Parse(number[2]) + 1).ToString();
                    number[1] = number[2] = "0";
                    break;
                }
                
                default: Debug.Log("WRONG BUILD TYPE."); return;
            }
            
            version = $"{number[0]}.{number[1]}.{number[2]}";
            PlayerSettings.bundleVersion = version;
        }
    }
}
#endif