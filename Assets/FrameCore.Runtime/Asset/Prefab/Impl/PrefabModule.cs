using System.Collections.Generic;
using UnityEngine;

namespace FrameCore.Runtime
{
    public class PrefabModule : IPrefabModule, IUpdater
    {        
        private readonly IAssetModule _assetModule;
        private readonly Dictionary<string, PrefabObject> _loadedDic;
        private readonly Dictionary<int, PrefabObject> _goInstanceIdDic;
        private readonly List<PrefabObject> _loadedAsyncList;

        public PrefabModule()
        {
            _assetModule = IocContainer.Resolve<IAssetModule>();
            _loadedDic = new Dictionary<string, PrefabObject>();
            _loadedAsyncList = new List<PrefabObject>();
            _goInstanceIdDic = new Dictionary<int, PrefabObject>();
        }

        public GameObject LoadSync(string assetName, Transform parent = null, bool instantiateInWorldSpace = false)
        {
            PrefabObject prefabObj;
            if (_loadedDic.ContainsKey(assetName))
            {
                prefabObj = _loadedDic[assetName];
                prefabObj.parent = parent;
                prefabObj.instantiateInWorldSpace = instantiateInWorldSpace;
                if (prefabObj.Asset == null)
                {
                    Debug.LogError($"[PrefabModule] Error LoadSync Asset is null : {prefabObj.Name}");
                    return null;
                }
                
                prefabObj.RefCount++;
                return InstanceAsset(prefabObj);
            }

            prefabObj = new PrefabObject
            {
                Name = assetName,
                RefCount = 1,
                Asset = _assetModule.LoadSync<GameObject>(assetName),
                parent = parent,
                instantiateInWorldSpace = instantiateInWorldSpace
            };
            _loadedDic.Add(assetName, prefabObj);
            return InstanceAsset(prefabObj);
        }

        public void LoadAsync(string assetName, PrefabLoadCallback callFun, Transform parent = null,
            bool instantiateInWorldSpace = false)
        {
            PrefabObject prefabObj = null;
            if (_loadedDic.ContainsKey(assetName))
            {
                prefabObj = _loadedDic[assetName];
                prefabObj.CallbackList.Add(callFun);
                prefabObj.RefCount++;
                if (prefabObj.Asset != null)
                    _loadedAsyncList.Add(prefabObj);
                return;
            }

            prefabObj = new PrefabObject()
            {
                Name = assetName,
                parent = parent,
                instantiateInWorldSpace = instantiateInWorldSpace,
            };
            prefabObj.CallbackList.Add(callFun);
            _loadedDic.Add(assetName, prefabObj);
            _assetModule.LoadAsync(assetName, (string name, Object obj) =>
                {
                    prefabObj.Asset = obj;
                    prefabObj.LockCallbackCount = prefabObj.CallbackList.Count;
                    DoInstanceAssetCallback(prefabObj);
                }
            );
        }

        public void RemoveCallback(string assetName, PrefabLoadCallback callFun)
        {
            if (callFun == null) 
                return;

            PrefabObject prefabObj = null;
            if (_loadedDic.ContainsKey(assetName))
                prefabObj = _loadedDic[assetName];

            if (prefabObj != null)
            {
                var index = prefabObj.CallbackList.IndexOf(callFun);
                if (index >= 0)
                {
                    prefabObj.RefCount--;
                    prefabObj.CallbackList.RemoveAt(index);
                    if (index < prefabObj.LockCallbackCount) // 说明是加载回调过程中解绑回调，需要降低lock个数
                    {
                        prefabObj.LockCallbackCount--;
                    }
                }

                if (prefabObj.RefCount < 0)
                {
                    Debug.LogError($"[PrefabModule] Error refCount : {prefabObj.Name}");
                    return;
                }

                if (prefabObj.RefCount == 0)
                {
                    _loadedDic.Remove(prefabObj.Name);
                    _assetModule.Unload(prefabObj.Asset);
                    prefabObj.Asset = null;
                }
            }
        }

        // 用于外部实例化，增加引用计数
        public void AddAssetRef(string assetName, GameObject gameObject)
        {
            if (!_loadedDic.ContainsKey(assetName))
                return;

            var prefabObj = _loadedDic[assetName];
            var instanceID = gameObject.GetInstanceID();
            if (_goInstanceIdDic.ContainsKey(instanceID))
            {
                Debug.LogError($"[PrefabModule] AddAssetRef error ! assetName:{assetName}");
                return;
            }

            prefabObj.RefCount++;
            prefabObj.GoInstanceIdSet.Add(instanceID);
            _goInstanceIdDic.Add(instanceID, prefabObj);
        }

        public void Destroy(GameObject obj)
        {
            if (obj == null) 
                return;

            var instanceID = obj.GetInstanceID();
            if (!_goInstanceIdDic.ContainsKey(instanceID)) // 非从本类创建的资源，直接销毁即可
            {
                Object.Destroy(obj);
                return;
            }

            var prefabObj = _goInstanceIdDic[instanceID];
            if (prefabObj.GoInstanceIdSet.Contains(instanceID)) // 实例化的GameObject
            {
                prefabObj.RefCount--;
                prefabObj.GoInstanceIdSet.Remove(instanceID);
                _goInstanceIdDic.Remove(instanceID);
                Object.Destroy(obj);
            }
            else
            {
                Debug.LogError($"[PrefabModule] Error Destroy : {prefabObj.Name}");
                return;
            }

            if (prefabObj.RefCount < 0)
            {
                Debug.LogError($"[PrefabModule] Error refCount : {prefabObj.Name}");
                return;
            }

            if (prefabObj.RefCount != 0) 
                return;
            
            _loadedDic.Remove(prefabObj.Name);
            _assetModule.Unload(prefabObj.Asset);
            prefabObj.Asset = null;
        }

        private void DoInstanceAssetCallback(PrefabObject prefabObject)
        {
            if (prefabObject.CallbackList.Count == 0) return;

            var count = prefabObject.LockCallbackCount;
            var callbackList = prefabObject.CallbackList.GetRange(0, count);
            prefabObject.LockCallbackCount = 0;
            prefabObject.CallbackList.RemoveRange(0, count);
            for (var i = 0; i < count; i++)
            {
                if (callbackList[i] != null)
                {
                    var newObj = InstanceAsset(prefabObject);
                    try
                    {
                        callbackList[i](prefabObject.Name, newObj);
                    }
                    catch (System.Exception e)
                    {
                        Debug.LogError(e);
                    }

                    //如果回调之后，节点挂在默认节点下，认为该节点无效，销毁
                    //if (newObj.transform.parent == assetParent.transform)
                    //Destroy(newObj);
                }
            }
        }

        private GameObject InstanceAsset(PrefabObject prefabObj)
        {
            var go = Object.Instantiate(prefabObj.Asset, prefabObj.parent, prefabObj.instantiateInWorldSpace) as GameObject;
            go.name = go.name.Replace("(Clone)", "");
            var instanceID = go.GetInstanceID();
            var obgInfo = go.AddComponent<ObjInfo>();
            if (obgInfo != null)
            {
                obgInfo.InstanceId = instanceID;
                obgInfo.AssetName = prefabObj.Name;
            }

            prefabObj.GoInstanceIdSet.Add(instanceID);
            _goInstanceIdDic.Add(instanceID, prefabObj);
            return go;
        }

        private void UpdateLoadedAsync()
        {
            if (_loadedAsyncList.Count == 0)
                return;

            var count = _loadedAsyncList.Count;
            for (var i = 0; i < count; i++)
            {
                _loadedAsyncList[i].LockCallbackCount = _loadedAsyncList[i].CallbackList.Count;
            }

            for (var i = 0; i < count; i++)
            {
                DoInstanceAssetCallback(_loadedAsyncList[i]);
            }

            _loadedAsyncList.RemoveRange(0, count);
        }
        
        public void Update()
        {
            UpdateLoadedAsync();
        }
    }
}
