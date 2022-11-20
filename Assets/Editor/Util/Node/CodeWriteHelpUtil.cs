using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.Experimental.SceneManagement;
using UnityEngine;
using FrameCore.Runtime;

namespace FrameCore.Editor
{
    public static class CodeWriteHelpUtil
    {
        // 代码生成所需要的程序集列表
        private static Assembly[] s_Assemblies;

        public static Assembly[] Assemblies
        {
            get
            {
                if (s_Assemblies == null)
                {
                    var config =
                        AssetDatabase.LoadAssetAtPath<CodeGenConfig>("Assets/PreResources/Config/CodeGenConfig.asset");
                    if (config == null)
                        config = Resources.Load<CodeGenConfig>("Config\\CodeGenConfig");

                    s_Assemblies = config.CodeGenRefAssemblies.Where(
                        r => !string.IsNullOrEmpty(r.name)
                    ).Select(
                        r => Assembly.Load(r.name)
                    ).ToArray();
                }

                return s_Assemblies;
            }
        }

        public static void WriteNameSpace(ObjectCollector objectCollector, HashSet<string> nameSpaceHashSet)
        {
            List<Object>.Enumerator enumValues = objectCollector.values.GetEnumerator();
            while (enumValues.MoveNext())
            {
                if (enumValues.Current == null) continue;
                if (enumValues.Current.GetType().Namespace != null)
                {
                    nameSpaceHashSet.Add($"using {enumValues.Current.GetType().Namespace};");
                }
            }
        }

        public static void WriteFields(CodeWriter writer, ObjectCollector objectCollector)
        {
            List<string>.Enumerator enumKeys = objectCollector.keys.GetEnumerator();
            List<Object>.Enumerator enumValues = objectCollector.values.GetEnumerator();
            while (enumKeys.MoveNext() && enumValues.MoveNext())
            {
                if (enumValues.Current == null) continue;
                CodeFieldGetter fieldGetter = CodeGetter(enumValues.Current);

                foreach (var code in fieldGetter.GetFieldCode(enumKeys.Current, enumValues.Current))
                {
                    writer.WriteLine(code);
                }
            }
        }

        // 写字段
        public static void WriteFields2(CodeWriter writer, ObjectCollector objectCollector)
        {
            List<string>.Enumerator enumKeys = objectCollector.keys.GetEnumerator();
            List<Object>.Enumerator enumValues = objectCollector.values.GetEnumerator();
            while (enumKeys.MoveNext() && enumValues.MoveNext())
            {
                if (enumValues.Current == null)
                    continue;

                CodeFieldGetter fieldGetter = CodeGetter(enumValues.Current);
                foreach (var code in fieldGetter.GetFieldCode1(enumKeys.Current, enumValues.Current))
                {
                    writer.WriteLine(code);
                }
            }
        }

        // 给字段赋值
        public static void WriteFields3(CodeWriter writer, ObjectCollector objectCollector)
        {
            List<string>.Enumerator enumKeys = objectCollector.keys.GetEnumerator();
            List<Object>.Enumerator enumValues = objectCollector.values.GetEnumerator();
            while (enumKeys.MoveNext() && enumValues.MoveNext())
            {
                if (enumValues.Current == null)
                    continue;

                CodeFieldGetter fieldGetter = CodeGetter(enumValues.Current);
                foreach (var code in fieldGetter.GetFieldCode2(enumKeys.Current, enumValues.Current))
                {
                    writer.WriteLine(code);
                }
            }
        }

        // 给字段赋值
        public static void WriteFields4(CodeWriter writer, ObjectCollector objectCollector)
        {
            List<string>.Enumerator enumKeys = objectCollector.keys.GetEnumerator();
            List<Object>.Enumerator enumValues = objectCollector.values.GetEnumerator();
            while (enumKeys.MoveNext() && enumValues.MoveNext())
            {
                if (enumValues.Current == null)
                    continue;

                CodeFieldGetter fieldGetter = CodeGetter(enumValues.Current);
                foreach (var code in fieldGetter.GetFieldCode3(enumKeys.Current, enumValues.Current))
                {
                    writer.WriteLine(code);
                }
            }
        }

        public static CodeFieldGetter CodeGetter(UnityEngine.Object ob)
        {
            var types = Assemblies.SelectMany(r => r.GetTypes()).ToArray();
            var genObjType = ob.GetType();
            GameObject go = ob as GameObject ?? (ob as Component).gameObject;
            // foreach (var type in types)
            // {
            //     var attribute = type.GetCustomAttribute<UICodeGenAttribute>();
            //     if (attribute != null && (attribute.type == genObjType))
            //     {
            //         CodeFieldGetter CodeFieldGetter = Activator.CreateInstance(type, new object[] { go }) as CodeFieldGetter;
            //         return CodeFieldGetter;
            //     }
            // }
            return new NormalFieldGetter(go);
        }

#pragma warning disable 0618

        public static string GetPrefabOriginalName(GameObject go)
        {
            var assetPath = GetPrefabAssetPath(go);
            return string.IsNullOrEmpty(assetPath) ? string.Empty : Path.GetFileNameWithoutExtension(assetPath);
        }

        public static string GetPrefabOriginalFolder(GameObject go)
        {
            var assetPath = GetPrefabAssetPath(go);
            return assetPath.Replace(".prefab", "").Replace("\\", "/");
        }

        public static string GetPrefabAssetPath(GameObject gameObject)
        {
            // Project中的Prefab是Asset不是Instance
            if (PrefabUtility.IsPartOfPrefabAsset(gameObject))
            {
                // 预制体资源就是自身
                return AssetDatabase.GetAssetPath(gameObject);
            }

            // Scene中的Prefab Instance是Instance不是Asset
            if (PrefabUtility.IsPartOfPrefabInstance(gameObject))
            {
                // 获取预制体资源
                var prefabAsset = PrefabUtility.GetCorrespondingObjectFromOriginalSource(gameObject);
                return UnityEditor.AssetDatabase.GetAssetPath(prefabAsset);
            }

            // PrefabMode中的GameObject既不是Instance也不是Asset
            var prefabStage = PrefabStageUtility.GetPrefabStage(gameObject);
            if (prefabStage != null)
            {
                // 预制体资源：prefabAsset = prefabStage.prefabContentsRoot
                return prefabStage.prefabAssetPath;
            }

            // 不是预制体
            return string.Empty;
        }

        public static string GetPrefabUIAssetPath(GameObject go)
        {
            var folder = GetPrefabOriginalFolder(go);
            var keyword = "UI/";
            return string.IsNullOrEmpty(folder)
                ? string.Empty
                : folder.Substring(folder.IndexOf(keyword) + keyword.Length);
        }

        public static string GetPrefabUIFolder(string product, GameObject go, bool removeLastFolder = false)
        {
            var folder = GetPrefabOriginalFolder(go);
            if (string.IsNullOrEmpty(folder))
                return string.Empty;

            var subFolder = folder.Substring(folder.IndexOf("UI/"));
            if (removeLastFolder)
            {
                var cnt = subFolder.LastIndexOf("/");
                if (cnt > 0)
                    subFolder = subFolder.Substring(0, cnt);
            }

            return $"Assets/{product}/{subFolder}";
        }

        public static string GetPrefabMapAssetPath(GameObject go)
        {
            var folder = GetPrefabOriginalFolder(go);
            var keyword = "Map/";
            return string.IsNullOrEmpty(folder)
                ? string.Empty
                : folder.Substring(folder.IndexOf(keyword) + keyword.Length);
        }

        public static string GetPrefabEntityAssetPath(GameObject go)
        {
            var folder = GetPrefabOriginalFolder(go);
            var keyword = "Entity/";
            return string.IsNullOrEmpty(folder)
                ? string.Empty
                : folder.Substring(folder.IndexOf(keyword) + keyword.Length);
        }

        public static string GetPrefabMapFolder(string product, string folderPrefix, GameObject go,
            bool removeLastFolder = false)
        {
            var folder = GetPrefabOriginalFolder(go);
            if (string.IsNullOrEmpty(folder))
                return string.Empty;

            var startIndex = folder.IndexOf(folderPrefix);
            if (startIndex == -1)
            {
                return string.Empty;
            }

            var subFolder = folder.Substring(startIndex);
            if (removeLastFolder)
            {
                var cnt = subFolder.LastIndexOf("/");
                if (cnt > 0)
                    subFolder = subFolder.Substring(0, cnt);
            }

            return $"Assets/{product}/{subFolder}";
        }
    }

#pragma warning restore 0618
    public class NormalFieldGetter : CodeFieldGetter
    {
        public NormalFieldGetter(GameObject go) : base(go)
        {
        }
    }
}
