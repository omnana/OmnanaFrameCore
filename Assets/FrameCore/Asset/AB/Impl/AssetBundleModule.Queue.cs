using System;
using System.Collections.Generic;
using UnityEngine;

namespace FrameCore.Runtime
{
    public partial class AssetBundleModule
    {
        private readonly List<AssetBundleObject> _tempList;
        private Dictionary<string, AssetBundleObject> _readyDic;
        private Dictionary<string, AssetBundleObject> _loadingDic;
        private Dictionary<string, AssetBundleObject> _loadedDic;
        private Dictionary<string, AssetBundleObject> _unloadDic;

        private void InitQueue()
        {
            _readyDic = new Dictionary<string, AssetBundleObject>();
            _loadingDic = new Dictionary<string, AssetBundleObject>();
            _loadedDic = new Dictionary<string, AssetBundleObject>();
            _unloadDic = new Dictionary<string, AssetBundleObject>();
        }

        private AssetBundleObject GetFromQueue(string abName)
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
                case AssetBundleState.None:
                    break;
                default:
                    return;
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
                default:
                    return;
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
                if (obj.DependLoadingCount != 0 || !(obj.Request is {isDone: true}))
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
    }
}