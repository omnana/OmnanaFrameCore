using System.Collections.Generic;
using UnityEngine;

namespace FrameCore.Runtime
{
    public class AssetModule : IAssetModule, IUpdater
    {
        private readonly IAssetBundleModule _assetBundleModule;
        private readonly IResourceModule _resourceModule;
        private readonly IEditorAssetModule _editorAssetModule;

        private readonly Dictionary<string, AssetObject> _loadingDic;
        private readonly Dictionary<string, AssetObject> _loadedDic;
        private readonly Dictionary<string, AssetObject> _unloadDic;
        private readonly Queue<PreloadAssetObject> _preloadedAsyncList; // 预加载队列
        private readonly List<AssetObject> _loadedAsyncList; // 异步加载队列，延迟回调
        private readonly Dictionary<int, AssetObject> _goInstanceIdDic; //创建的实例对应的asset
        private readonly List<AssetObject> _tempLoaded;

        private int _loadingIntervalCount;
        private Dictionary<string, string> _assetConfig;

        public AssetModule()
        {
            if (!AssetHelper.IsAbMode)
            {
                _editorAssetModule = IocContainer.Resolve<IEditorAssetModule>();
            }
            else
            {
                _assetBundleModule = IocContainer.Resolve<IAssetBundleModule>();
            }

            _resourceModule = IocContainer.Resolve<IResourceModule>();
            _loadingDic = new Dictionary<string, AssetObject>();
            _loadedDic = new Dictionary<string, AssetObject>();
            _unloadDic = new Dictionary<string, AssetObject>();
            _loadedAsyncList = new List<AssetObject>();
            _preloadedAsyncList = new Queue<PreloadAssetObject>();
            _goInstanceIdDic = new Dictionary<int, AssetObject>();
            _tempLoaded = new List<AssetObject>();
        }

        public void Load()
        {
            if (!AssetHelper.IsAbMode)
            {
                _editorAssetModule.SetProduct();
            }
            else
            {
                _assetBundleModule.LoadManifest();
                LoadAssetConfig();
            }
        }

        public bool IsFileExist(string assetName)
        {
            if (!AssetHelper.IsAbMode)
            {
                return _editorAssetModule.IsFileExist(assetName) || _resourceModule.IsFileExist(assetName);
            }

            return _resourceModule.IsFileExist(assetName) || IsAssetExist(assetName);
        }

        public T LoadSync<T>(string assetName) where T : Object
        {
            assetName = assetName.ToLower();
            if (!IsFileExist(assetName))
            {
                Debug.LogError($"[AssetModule] Error : {assetName} is no exist!!");
                return null;
            }

            var assetObj = GetAssetObj(assetName) ?? new AssetObject() {Name = assetName};
            if (assetObj.Status == AssetObjStatus.Unload)
            {
                assetObj.AddRefCount();
                EnQueue(AssetObjStatus.Loaded, assetObj);
                return assetObj.Asset as T;
            }

            if (assetObj.Status == AssetObjStatus.Loading)
            {
                assetObj.ForceSync();
                assetObj.DoCallback();
                EnQueue(AssetObjStatus.Loaded, assetObj);
                return assetObj.Asset as T;
            }

            if (assetObj.Status == AssetObjStatus.Loaded)
            {
                assetObj.AddRefCount();
                return assetObj.Asset as T;
            }

            if (!AssetHelper.IsAbMode)
            {
                if (_resourceModule.IsFileExist(assetName))
                {
                    assetObj.IsAbLoad = false;
                    assetObj.Asset = _resourceModule.LoadSync<T>(assetName);
                }
                else
                {
                    assetObj.Asset = _editorAssetModule.LoadSync<T>(assetName);
                }
            }
            else
            {
                if (_resourceModule.IsFileExist(assetName))
                {
                    assetObj.IsAbLoad = false;
                    assetObj.Asset = _resourceModule.LoadSync<T>(assetName);
                }
                else if (IsFileExist(assetName))
                {
                    assetObj.IsAbLoad = true;
                    var abName = GetAssetAbName(assetName);
                    var ab = _assetBundleModule.LoadSync(abName).AssetBundle;
                    assetObj.Asset = ab.LoadAsset(GetAssetName(assetName));
                }
            }

            if (assetObj.Asset == null)
            {
                EnQueue(AssetObjStatus.None, assetObj);
                Debug.LogError($"[AssetModule] Error : {assetObj.Name} is no exist!!");
                return null;
            }

            assetObj.InstanceID = assetObj.Asset.GetInstanceID();
            _goInstanceIdDic.Add(assetObj.InstanceID, assetObj);
            assetObj.AddRefCount();
            EnQueue(AssetObjStatus.Loaded, assetObj);
            return assetObj.Asset as T;
        }

        public void LoadAsync(string assetName, AssetsLoadCallback callFun)
        {
            assetName = assetName.ToLower();
            var assetObj = GetAssetObj(assetName) ?? new AssetObject() {Name = assetName};
            EnQueue(AssetObjStatus.Loading, assetObj);
            if (!AssetHelper.IsAbMode)
            {
                if (_resourceModule.IsFileExist(assetName))
                {
                    assetObj.IsAbLoad = false;
                    assetObj.Asset = _resourceModule.LoadSync<Object>(assetName);
                }
                else
                {
                    assetObj.Asset = _editorAssetModule.LoadSync<Object>(assetName);
                }
            }
            else
            {
                if (IsFileExist(assetName))
                {
                    assetObj.IsAbLoad = true;
                    var abName = GetAssetAbName(assetName);
                    _assetBundleModule.LoadAsync(abName, (ab) =>
                    {
                        if (assetObj.Status == AssetObjStatus.Loading && assetObj.Request == null &&
                            assetObj.Asset == null)
                        {
                            assetObj.Request = ab.LoadAssetAsync(GetAssetName(assetName));
                        }
                    });
                }
                else if (_resourceModule.IsFileExist(assetName))
                {
                    assetObj.IsAbLoad = false;
                    assetObj.Request = _resourceModule.LoadAsync(assetName);
                }
            }

            assetObj.CallbackList.Add(callFun);
            if (assetObj.Status == AssetObjStatus.Loaded)
            {
                _loadedAsyncList.Add(assetObj);
            }
        }

        /// <summary>
        /// 预加载，isWeak弱引用，true为使用过后会销毁，为false将不会销毁，慎用
        /// </summary>
        /// <param name="assetName"></param>
        /// <param name="isWeak"></param>
        public void PreLoad(string assetName, bool isWeak = true)
        {
            assetName = assetName.ToLower();
            var assetObj = GetAssetObj(assetName);
            if (assetObj != null)
            {
                assetObj.IsWeak = isWeak;
                if (isWeak && assetObj.RefCount == 0)
                {
                    EnQueue(AssetObjStatus.Unload, assetObj);
                }

                return;
            }

            _preloadedAsyncList.Enqueue(new PreloadAssetObject()
            {
                Name = assetName,
                IsWeak = isWeak
            });
        }

        public void Unload(Object obj)
        {
            if (obj == null)
                return;

            var instanceID = obj.GetInstanceID();
            if (!_goInstanceIdDic.ContainsKey(instanceID) && obj is GameObject) // 非从本类创建的资源，直接销毁即可
            {
                Object.Destroy(obj);
                return;
            }

            if (!_goInstanceIdDic.ContainsKey(instanceID))
            {
                Debug.LogError($"AssetsLoadMgr Destroy error : obj is no exist!! assetName : {obj.name}");
                return;
            }

            var assetObj = _goInstanceIdDic[instanceID];
            if (assetObj.InstanceID == instanceID)
            {
                assetObj.ReduceRefCount();
            }
            else // error
            {
                Debug.LogError($"AssetsLoadMgr Destroy error ! assetName:{assetObj.Name}");
                return;
            }

            if (assetObj.RefCount < 0)
            {
                Debug.LogError($"AssetsLoadMgr Destroy refCount error ! assetName:{assetObj.Name}");
                return;
            }

            if (assetObj.RefCount != 0 || _unloadDic.ContainsKey(assetObj.Name))
                return;

            assetObj.UnloadTick = AssetHelper.UNLOAD_DELAY_TICK_BASE + _unloadDic.Count;
            EnQueue(AssetObjStatus.Unload, assetObj);
        }

        public string GetAssetAbName(string assetName)
        {
            assetName = assetName.ToLower();
            if (!_assetConfig.ContainsKey(assetName))
            {
                FrameDebugger.LogError($"未找到asset:{assetName}的ab！！！");
                return string.Empty;
            }

            return _assetConfig[assetName];
        }

        #region 私有函数

        private void LoadAssetConfig()
        {
            var assetConfigPath = AssetBundleHelper.GetAbPath("assetconfig");
            var ab = AssetBundle.LoadFromFile(assetConfigPath);
            if (ab == null)
            {
                FrameDebugger.LogError($"未加载到assetConfig.bytes!!，请重新打ab");
            }

            var json = ab.LoadAsset<TextAsset>("assetconfig.bytes");
            _assetConfig = LitJson.JsonMapper.ToObject<Dictionary<string, string>>(json.text);
            ab.Unload(true);
        }

        private string GetAssetName(string assetName)
        {
            if (!_assetConfig.ContainsKey(assetName))
            {
                FrameDebugger.LogError($"未找到asset:{assetName}！！！");
                return string.Empty;
            }

            return $"assets/rawresources/{assetName}";
        }

        private bool IsAssetExist(string assetName)
        {
            return _assetConfig.ContainsKey(assetName);
        }

        private AssetObject GetAssetObj(string assetName)
        {
            if (_loadingDic.ContainsKey(assetName))
                return _loadedDic[assetName];
            if (_loadedDic.ContainsKey(assetName))
                return _loadedDic[assetName];

            return _unloadDic.ContainsKey(assetName) ? _unloadDic[assetName] : null;
        }

        private void EnQueue(AssetObjStatus status, AssetObject obj)
        {
            DeQueue(obj);
            switch (status)
            {
                case AssetObjStatus.Loading:
                    obj.Status = AssetObjStatus.Loading;
                    _loadingDic.Add(obj.Name, obj);
                    break;
                case AssetObjStatus.Loaded:
                    obj.Status = AssetObjStatus.Loaded;
                    _loadedDic.Add(obj.Name, obj);
                    break;
                case AssetObjStatus.Unload:
                    obj.Status = AssetObjStatus.Unload;
                    _unloadDic.Add(obj.Name, obj);
                    break;
            }
        }

        private void DeQueue(AssetObject obj)
        {
            var abName = obj.Name;
            _loadingDic.Remove(abName);
            _loadedDic.Remove(abName);
            _unloadDic.Remove(abName);
            obj.Status = AssetObjStatus.None;
        }

        private void DoUnLoad(AssetObject assetObject)
        {
            if (!AssetHelper.IsAbMode)
            {
                _editorAssetModule.Unload(assetObject.Asset);
            }
            else
            {
                if (assetObject.IsAbLoad)
                    _assetBundleModule.Unload(GetAssetAbName(assetObject.Name));
                else
                    _resourceModule.Unload(assetObject.Asset);
            }

            assetObject.Asset = null;
            if (_goInstanceIdDic.ContainsKey(assetObject.InstanceID))
            {
                _goInstanceIdDic.Remove(assetObject.InstanceID);
            }
            else
            {
                FrameDebugger.LogError($"[AssetModule] 资源对应go不存在:{assetObject.Name}！！！");
            }
        }

        // 预加载
        private void UpdatePreload()
        {
            // 加载队列空闲才需要预加载
            if (_loadingDic.Count > 0 || _preloadedAsyncList.Count == 0)
                return;

            // 从队列支取处取出一个，异步加载，直到加载完，在取下一个
            var plAssetObj = _preloadedAsyncList.Peek();
            var assetObj = GetAssetObj(plAssetObj.Name);
            if (assetObj != null)
            {
                assetObj.IsWeak = plAssetObj.IsWeak;
            }
            else
            {
                LoadAsync(plAssetObj.Name, null);
            }

            if (assetObj?.Status == AssetObjStatus.Loaded)
            {
                _preloadedAsyncList.Dequeue();
            }
        }

        private void UpdateLoadAsync()
        {
            if (_loadedAsyncList.Count == 0)
                return;

            var count = _loadedAsyncList.Count;
            for (var i = 0; i < count; i++)
            {
                //先锁定回调数量，保证异步成立
                _loadedAsyncList[i].LockCallbackCount = _loadedAsyncList[i].CallbackList.Count;
            }

            for (int i = 0; i < count; i++)
            {
                _loadedAsyncList[i].DoCallback();
            }

            _loadedAsyncList.RemoveRange(0, count);
            if (_loadingDic.Count != 0 || _loadingIntervalCount <= AssetHelper.LOADING_INTERVAL_MAX_COUNT)
                return;

            //在连续的大量加载后，强制调用一次gc
            _loadingIntervalCount = 0;
            Resources.UnloadUnusedAssets();
            System.GC.Collect();
        }

        private void UpdateLoading()
        {
            if (_loadingDic.Count == 0)
                return;

            //检测加载完的
            _tempLoaded.Clear();
            foreach (var assetObj in _loadingDic.Values)
            {
                if (!AssetHelper.IsAbMode)
                {
                    if (assetObj.Asset != null)
                    {
                        assetObj.InstanceID = assetObj.Asset.GetInstanceID();
                        _goInstanceIdDic.Add(assetObj.InstanceID, assetObj);
                        _tempLoaded.Add(assetObj);
                    }
                }
                else
                {
                    if (assetObj.Request.isDone)
                    {
                        assetObj.Asset = assetObj.GetAsyncAsset();
                        assetObj.InstanceID = assetObj.Asset.GetInstanceID();
                        _goInstanceIdDic.Add(assetObj.InstanceID, assetObj);
                        assetObj.Request = null;
                        _tempLoaded.Add(assetObj);
                    }
                }
            }

            //回调中有可能对loadingList进行操作，先移动
            foreach (var assetObj in _tempLoaded)
            {
                EnQueue(AssetObjStatus.Loaded, assetObj);
                _loadingIntervalCount++; //统计本轮加载的数量
                // 先锁定回调数量，保证异步成立
                assetObj.LockCallbackCount = assetObj.CallbackList.Count;
            }

            foreach (var assetObj in _tempLoaded)
            {
                assetObj.DoCallback();
            }
        }

        private void UpdateUnload()
        {
            //遍历卸载，延迟卸载
            if (_unloadDic.Count == 0)
                return;

            _tempLoaded.Clear();
            foreach (var assetObj in _unloadDic.Values)
            {
                if (assetObj.IsWeak && assetObj.RefCount == 0 && assetObj.CallbackList.Count == 0)
                {
                    if (assetObj.UnloadTick < 0) // 引用计数为0，且没有需要回调的函数，销毁
                    {
                        DoUnLoad(assetObj);
                        _tempLoaded.Add(assetObj);
                    }
                    else
                    {
                        assetObj.UnloadTick--;
                    }
                }

                if (assetObj.RefCount > 0 || !assetObj.IsWeak)
                {
                    _tempLoaded.Add(assetObj); // 引用计数增加（销毁期间有加载）
                }
            }

            foreach (var assetObj in _tempLoaded)
            {
                if (assetObj.Status == AssetObjStatus.Unload)
                {
                    assetObj.Status = AssetObjStatus.None;
                    _unloadDic.Remove(assetObj.Name);
                }
            }
        }

        #endregion

        public void Update()
        {
            UpdatePreload();
            UpdateLoadAsync();
            UpdateLoading();
            UpdateUnload();
        }
    }
}
