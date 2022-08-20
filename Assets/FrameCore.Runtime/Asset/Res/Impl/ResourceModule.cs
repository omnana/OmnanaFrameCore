using System.Collections.Generic;
using FrameCore.Util;
using UnityEngine;

namespace FrameCore.Runtime
{
    public class ResourceModule : IResourceModule
    {
        private readonly Dictionary<string, string> _realNameDic;
        
        public ResourceModule()
        {
            _realNameDic = new Dictionary<string, string>();
            LoadConfig();
        }

        public bool IsFileExist(string assetName)
        {
            return _realNameDic.ContainsKey(assetName);
        }

        public ResourceRequest LoadAsync(string assetName)
        {
            if (IsFileExist(assetName))
                return Resources.LoadAsync(_realNameDic[assetName]);

            Debug.LogError($"[ResourceModule] Error : {assetName} is no exist");
            return null;

        }

        public T LoadSync<T>(string assetName) where T : Object
        {
            if (IsFileExist(assetName))
                return Resources.Load<T>(_realNameDic[assetName]);

            Debug.LogError($"[ResourceModule] Error : {assetName} is no exist");
            return null;
        }

        public void Unload(Object asset)
        {
            if (asset is GameObject)
            {
                return;
            }

            Resources.UnloadAsset(asset);
        }

        private void LoadConfig()
        {
            var fileListTextAsset = Resources.Load<TextAsset>("FileList");
            if (fileListTextAsset == null)
            {
                Debug.LogWarning("[ResourceModule] FileList is no exist!!");
                return;
            }

            var array = fileListTextAsset.text.Split('|');
            foreach (var a in array)
            {
                var realName = a.Replace("/", "\\").Substring(0, a.LastIndexOf("."));
                _realNameDic.Add(a, realName);
            }
        }
    }
}
