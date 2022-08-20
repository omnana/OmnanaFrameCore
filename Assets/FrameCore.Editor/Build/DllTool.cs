using System;
using System.IO;
using FrameCore.Util;
using UnityEditor;
using UnityEditor.Build.Player;
using UnityEngine;

namespace FrameCore.Editor
{
    public class DllTool
    {
        [MenuItem("Tools/CompileDll/Win64")]
        public static void CompileDllWin64()
        {
            var target = BuildTarget.StandaloneWindows64;
            CompileDll(GetDllBuildOutputDirByTarget(target), target);
        }

        public static string DllBuildOutputDir => Path.GetFullPath($"{Application.dataPath}/../Temp/Ideal/build");

        public static string GetDllBuildOutputDirByTarget(BuildTarget target)
        {
            return $"{DllBuildOutputDir}/{target}";
        }

        private static void CompileDll(string buildDir, BuildTarget target)
        {
            var btGroup = BuildPipeline.GetBuildTargetGroup(target);
            var scriptCompilationSettings = new ScriptCompilationSettings
            {
                group = btGroup,
                target = target,
                options = ScriptCompilationOptions.DevelopmentBuild
            };
            PlayerBuildInterface.CompilePlayerScripts(scriptCompilationSettings, buildDir);
        }

        public static void CopyDll2Resource(string dllName, string destDir)
        {
            try
            {
                if (!DirectoryUtil.Exist(destDir))
                {
                    DirectoryUtil.CreateDirectory(destDir);
                }

                var dllPath = GetDllBuildOutputDirByTarget(BuildTarget.StandaloneWindows64) + $"/{dllName}.dll";
                var dllBytesPath = destDir + $"/{dllName}.dll.bytes";
                File.Copy(dllPath, dllBytesPath, true);
                Debug.Log($"hot fix dll : {dllBytesPath}");
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }
        }

        public static void CopyScriptAssembliesDll2Resource(string dllName, string destDir)
        {
            try
            {
                if (!DirectoryUtil.Exist(destDir))
                {
                    DirectoryUtil.CreateDirectory(destDir);
                }
                var dllPath = Environment.CurrentDirectory + $"\\Library\\ScriptAssemblies\\{dllName}.dll";
                var dllBytesPath = destDir + $"/{dllName}.dll.bytes";
                File.Copy(dllPath, dllBytesPath, true);
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
        }
    }
}
