using System.Collections.Generic;
using FrameCore.Utility;
using UnityEngine;

namespace FrameCore.Runtime
{
    /// <summary>
    /// 
    /// </summary>
    public class AssetBundleModule : IAssetBundleModule, IUpdater
    {
        private readonly List<AssetBundleObject> _tempList;
        private readonly Dictionary<string, AssetBundleObject> _readyDic;
        private readonly Dictionary<string, AssetBundleObject> _loadingDic;
        private readonly Dictionary<string, AssetBundleObject> _loadedDic;
        private readonly Dictionary<string, AssetBundleObject> _unloadDic;
        private Dictionary<string, string[]> _dependsDic;
            
        public AssetBundleModule()
        {
            _readyDic = new Dictionary<string, AssetBundleObject>();
            _loadingDic = new Dictionary<string, AssetBundleObject>();
            _loadedDic = new Dictionary<string, AssetBundleObject>();
            _unloadDic = new Dictionary<string, AssetBundleObject>();
            _dependsDic = new Dictionary<string, string[]>();
            _tempList = new List<AssetBundleObject>();
        }

        public void LoadManifest()
        {
            _dependsDic.Clear();
            _dependsDic = new Dictionary<string, string[]>();
            var manifestFile = AssetBundleHelper.GetManifestPath();
            if (!FileUtility.Exists(manifestFile))
            {
                Debug.LogError($"{manifestFile} is no exist!!!");
                return;
            }

            // var ab = AssetBundle.LoadFromMemory(File.ReadAllBytes(manifestFile));
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
                var dps = manifest.GetAllDependencies(assetName);
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
        /// <param name="assetName"></param>
        /// <returns></returns>
        public AssetBundleObject LoadSync(string assetName)
        {
            var curName = assetName.ToLower();
            var abObj = GetAbObj(curName) ?? CreateAbObj(curName);
            switch (abObj.State)
            {
                case AssetBundleState.Loading:
                case AssetBundleState.Ready:
                {
                    abObj.DoDependsRef();
                    abObj.ForceSync();
                    EnQueue(AssetBundleState.Loaded, abObj);
                    return abObj;
                }
                case AssetBundleState.Loaded:
                    abObj.DoDependsRef();
                    EnQueue(AssetBundleState.Loaded, abObj);
                    return abObj;
            }

            abObj.AddRefCount();
            var abPath = AssetBundleHelper.GetAbPath(curName);
            abObj.AssetBundle = AssetBundle.LoadFromFile(abPath);
            foreach (var dp in abObj.Depends)
            {
                LoadSync(dp.Name);
            }

            EnQueue(AssetBundleState.Loaded, abObj);
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
            var abObj = GetAbObj(curName) ?? CreateAbObj(curName);
            if (abObj.State == AssetBundleState.Loaded)
            {
                abObj.DoDependsRef();
                callFunc?.Invoke(abObj.AssetBundle);
                return;
            }

            abObj.AddRefCount();
            abObj.RegisterCallback(callFunc);
            EnQueue(AssetBundleState.Ready, abObj);
            var dps = abObj.Depends;
            foreach (var dp in dps)
            {
                LoadAsync(dp.Name, ab =>
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
            var abObj = GetAbObj(curName);
            if (abObj == null)
            {
                Debug.LogError($"[ab Unload] Error : ab : {curName} is no exist!!!");
                return;
            }

            if (abObj.RefCount == 0)
            {
                Debug.LogError($"[ab Unload] Error : ab : {curName} is RefCount == 0!!!");
                return;
            }

            abObj.ReduceRefCount();
            foreach (var dpObj in abObj.Depends)
            {
                Unload(dpObj.Name);
            }

            if (abObj.RefCount == 0)
            {
                EnQueue(AssetBundleState.Unload, abObj);
            }
        }

        #region 私有函数

        private AssetBundleObject CreateAbObj(string assetName)
        {
            var obj = new AssetBundleObject(assetName);
            if (!_dependsDic.ContainsKey(assetName))
                return obj;

            var dps = _dependsDic[assetName];
            if (dps == null || dps.Length <= 0)
                return obj;

            // 异步加载，需要加载完所有依赖项
            obj.DependLoadingCount = dps.Length;
            foreach (var dp in dps)
            {
                obj.Depends.Add(CreateAbObj(dp));
            }

            return obj;
        }

        private void DoUnLoad(AssetBundleObject abObj)
        {
            // 这里用true，卸载Asset内存，实现指定卸载
            if (abObj.AssetBundle == null)
            {
                Debug.LogError($"[Ab Unload] Error ! assetName : {abObj.Name}");
                return;
            }

            abObj.AssetBundle.Unload(true);
            abObj.AssetBundle = null;
        }

        private AssetBundleObject GetAbObj(string abName)
        {
            if (_readyDic.ContainsKey(abName))
                return _readyDic[abName];
            
            if (_loadingDic.ContainsKey(abName))
                return _loadingDic[abName];
            
            if (_loadedDic.ContainsKey(abName))
                return _loadedDic[abName];
            
            if (_unloadDic.ContainsKey(abName))
                return _unloadDic[abName];
            
            return null;
        }

        private void EnQueue(AssetBundleState state, AssetBundleObject abObj)
        {
            DeQueue(abObj);
            switch (state)
            {
                case AssetBundleState.Ready:
                    abObj.State = AssetBundleState.Ready;
                    _readyDic.Add(abObj.Name, abObj);
                    break;
                case AssetBundleState.Loading:
                    abObj.State = AssetBundleState.Loading;
                    _loadingDic.Add(abObj.Name, abObj);
                    break;
                case AssetBundleState.Loaded:
                    abObj.State = AssetBundleState.Loaded;
                    _loadedDic.Add(abObj.Name, abObj);
                    break;
                case AssetBundleState.Unload:
                    abObj.State = AssetBundleState.Unload;
                    _unloadDic.Add(abObj.Name, abObj);
                    break;
            }
        }

        private void DeQueue(AssetBundleObject abObj)
        {
            var abName = abObj.Name;
            switch (abObj.State)
            {
                case AssetBundleState.Ready:
                    _readyDic.Remove(abName);
                    break;
                case AssetBundleState.Loading:
                    _loadingDic.Remove(abName);
                    break;
                case AssetBundleState.Loaded:
                    _loadedDic.Remove(abName);
                    break;
                case AssetBundleState.Unload:
                    _unloadDic.Remove(abName);
                    break;
                case AssetBundleState.None:
                    break;
            }

            abObj.State = AssetBundleState.None;
        }

        private void UpdateReady()
        {
            if (_readyDic.Count == 0 || _loadingDic.Count > AssetBundleHelper.Max_Loading_Count)
                return;

            _tempList.Clear();
            var readyValues = _readyDic.Values;
            foreach (var abObj in readyValues)
            {
                var abPath = AssetBundleHelper.GetAbPath(abObj.Name);
                abObj.Request = AssetBundle.LoadFromFileAsync(abPath);
                _tempList.Add(abObj);
            }

            foreach (var t in _tempList)
            {
                EnQueue(AssetBundleState.Loading, t);
            }
        }

        private void UpdateLoading()
        {
            if (_loadingDic.Count == 0)
                return;

            _tempList.Clear();
            var loadings = _loadingDic.Values;
            foreach (var obj in loadings)
            {
                if (obj.DependLoadingCount != 0 || !obj.Request.isDone)
                    continue;

                obj.AssetBundle = obj.Request.assetBundle;
                _tempList.Add(obj);
            }

            for (int i = 0, count = _tempList.Count; i < count; i++)
            {
                var t = _tempList[i];
                t.PlayCallback();
                EnQueue(AssetBundleState.Loaded, t);
            }
        }

        private void UpdateUnLoad()
        {
            if (_unloadDic.Count == 0)
                return;

            _tempList.Clear();
            var unloads = _unloadDic.Values;
            foreach (var obj in unloads)
            {
                if (obj.RefCount != 0 || obj.AssetBundle == null)
                    continue;

                DoUnLoad(obj);
                _tempList.Add(obj);
            }

            foreach (var t in _tempList)
            {
                EnQueue(AssetBundleState.None, t);
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