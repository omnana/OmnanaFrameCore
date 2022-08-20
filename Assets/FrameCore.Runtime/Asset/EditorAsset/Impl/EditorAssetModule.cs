using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace FrameCore.Runtime
{
    public class EditorAssetModule : IEditorAssetModule
    {
        private readonly HashSet<string> _resourceHs;

        public EditorAssetModule()
        {
            _resourceHs = new HashSet<string>();
        }

        public void SetProduct()
        {
            var resFilePrefix = "RawResources/";
            var path = $"{Application.dataPath}/{resFilePrefix}";
            var files = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                if (file.EndsWith(".meta"))
                    continue;

                var name = file.Substring(file.IndexOf(resFilePrefix) + resFilePrefix.Length).Replace("\\", "/").ToLower();
                if (!_resourceHs.Contains(name))
                {
                    _resourceHs.Add(name);
                }
            }
        }

        public bool IsFileExist(string assetName)
        {
            return _resourceHs.Contains(assetName);
        }

        public T LoadSync<T>(string assetName) where T : Object
        {
            if (!_resourceHs.Contains(assetName))
            {
                Debug.LogError($"[EditorAssetModule] Error : {assetName} is no exist !!");
                return null;
            }

#if UNITY_EDITOR
            return AssetDatabase.LoadAssetAtPath<T>($"Assets/RawResources/{assetName}");
#else
            return null;
#endif
        }

        public void Unload(Object asset)
        {
            if (asset is null)
                return;

            if (asset is GameObject)
            {
                // Object.DestroyImmediate(asset);
            }
            else
            {
                Resources.UnloadAsset(asset);
            }
        }
    }
}
