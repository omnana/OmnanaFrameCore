using System.Collections.Generic;
using UnityEngine;

namespace FrameCore.Runtime
{
    /// <summary>
    /// ab管理器
    /// </summary>
    public partial class AssetBundleModule : IAssetBundleModule, IUpdater
    {
        private readonly Dictionary<string, string[]> _dependsDic;

        public AssetBundleModule()
        {
            _dependsDic = new Dictionary<string, string[]>();
            _tempList = new List<AssetBundleObject>();
        }

        public void LoadManifest()
        {
            _dependsDic.Clear();
            InitQueue();
            var manifestFile = AssetBundleHelper.GetManifestPath();
            var ab = AssetBundle.LoadFromFile(manifestFile);
            if (ab == null)
            {
                Debug.LogError("LoadManifest ab NULL error !");
                return;
            }

            var asset = ab.LoadAsset("AssetBundleManifest");
            var manifest = asset as AssetBundleManifest;
            if (manifest == null)
            {
                Debug.LogError("LoadManifest ab NULL error !");
                return;
            }

            foreach (var assetName in manifest.GetAllAssetBundles())
            {
                var dps = manifest.GetDirectDependencies(assetName);
                _dependsDic.Add(assetName, dps);
            }

            ab.Unload(true);
        }

        public bool IsFileExist(string assetName)
        {
            return _dependsDic.ContainsKey(assetName);
        }

        /// <summary>
        /// 同步接口
        /// </summary>
        /// <param name="abName"></param>
        /// <returns></returns>
        public AssetBundleObject LoadSync(string abName)
        {
            var curName = abName.ToLower();
            var abObj = GetAbObj(curName);
            switch (abObj.State)
            {
                case AssetBundleState.Loading:
                case AssetBundleState.Ready:
                {
                    DoDependForceSync(abObj);
                    DoDependRef(abObj);
                    return abObj;
                }
                case AssetBundleState.Loaded:
                    DoDependRef(abObj);
                    return abObj;
            }

            abObj.AddRefCount();
            var abPath = AssetBundleHelper.GetAbPath(curName);
            abObj.AssetBundle = AssetBundle.LoadFromFile(abPath);
            EnQueue(AssetBundleState.Loaded, abObj);
            foreach (var dp in abObj.Depends)
            {
                LoadSync(dp);
            }

            return abObj;
        }

        /// <summary>
        /// 异步接口
        /// </summary>
        /// <param name="abName"></param>
        /// <param name="callFunc"></param>
        public void LoadAsync(string abName, AssetBundleLoadCallBack callFunc)
        {
            var curName = abName.ToLower();
            var abObj = GetAbObj(curName);
            if (abObj.State == AssetBundleState.Loaded)
            {
                DoDependRef(abObj);
                callFunc?.Invoke(abObj.AssetBundle);
                return;
            }

            abObj.AddRefCount();
            abObj.RegisterCallback(callFunc);
            EnQueue(AssetBundleState.Ready, abObj);
            var dps = abObj.Depends;
            foreach (var dp in dps)
            {
                LoadAsync(dp, ab =>
                {
                    if (abObj.DependLoadingCount == 0)
                        return;

                    abObj.DependLoadingCount--;
                });
            }
        }

        public void Unload(string abName)
        {
            var curName = abName.ToLower();
            var abObj = GetFromQueue(curName);
            if (abObj == null)
            {
                FrameDebugger.LogError($"[ab Unload] Error : ab : {curName} is no exist!!!");
                return;
            }

            if (abObj.RefCount == 0)
            {
                FrameDebugger.LogError($"[ab Unload] Error : ab : {curName} is RefCount == 0!!!");
                return;
            }

            abObj.ReduceRefCount();
            foreach (var dp in abObj.Depends)
            {
                Unload(dp);
            }

            if (abObj.RefCount == 0)
            {
                EnQueue(AssetBundleState.Unload, abObj);
            }
        }

        #region 私有函数

        private AssetBundleObject GetAbObj(string abName)
        {
            var abObj = GetFromQueue(abName);
            if (abObj != null)
                return abObj;

            abObj = new AssetBundleObject(abName);
            if (_dependsDic.ContainsKey(abName))
            {
                var dps = _dependsDic[abName];
                if (dps == null || dps.Length == 0)
                    return abObj;

                abObj.DependLoadingCount = dps.Length; // 异步加载，需要加载完所有依赖项
                foreach (var dp in dps)
                {
                    abObj.Depends.Add(dp);
                }
            }

            return abObj;
        }

        private void DoUnLoad(AssetBundleObject abObj)
        {
            // 这里用true，卸载Asset内存，实现指定卸载
            if (abObj.AssetBundle == null)
            {
                FrameDebugger.LogError($"[Ab Unload] Error ! assetName : {abObj.Name}");
                return;
            }

            abObj.AssetBundle.Unload(true);
            abObj.AssetBundle = null;
        }

        private void DoDependRef(AssetBundleObject abObj)
        {
            abObj.AddRefCount();
            foreach (var dp in abObj.Depends)
            {
                var dpAbObj = GetFromQueue(dp);
                if (dpAbObj != null)
                {
                    DoDependRef(dpAbObj);
                }
                else
                {
                    FrameDebugger.LogError($"Error: ab {abObj.Name}'s dpAB {dp} is remove");
                }
            }
        }

        private void DoDependForceSync(AssetBundleObject abObj)
        {
            abObj.ForceSync();
            EnQueue(AssetBundleState.Loaded, abObj);
            foreach (var dp in abObj.Depends)
            {
                var dpAbObj = GetFromQueue(dp);
                if (dpAbObj.State == AssetBundleState.Loading)
                    DoDependForceSync(dpAbObj);
            }
        }

        #endregion

        public void Update()
        {
            UpdateLoading();
            UpdateReady();
            UpdateUnLoad();
        }
    }
}