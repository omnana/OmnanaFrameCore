using System;
using FrameCore.Runtime;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace FrameCore.Editor
{
    public class AppBuilder
    {
        [MenuItem("Omnana/打包App/Win64")]
        public static void BuildWinPlayer()
        {
            AbTool.GenerateResourceConfig();
            var product = GameConfigHelper.GetProduct();
            var buildPlayerOptions = new BuildPlayerOptions
            {
                scenes = new[] {"Assets/Scenes/StartUp.unity"},
                locationPathName =
                    $"{Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory)}\\{product}_{DateTime.Now.Ticks}\\{product}.exe",
                target = BuildTarget.StandaloneWindows64,
                options = BuildOptions.None
            };

            BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
            BuildSummary summary = report.summary;
            if (summary.result == BuildResult.Succeeded)
            {
                Debug.Log("Build succeeded: " + summary.totalSize + " bytes");
            }

            if (summary.result == BuildResult.Failed)
            {
                Debug.Log("Build failed");
            }
        }
    }
}
