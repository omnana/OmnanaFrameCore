using System.Collections.Generic;
using UnityEngine;

namespace FrameCore.Runtime
{
    public delegate void AssetBundleLoadCallBack(AssetBundle ab);
    public sealed class AssetBundleObject
    {
        public AssetBundleState State { get; set; } = AssetBundleState.None;
        /// <summary> hash标识符 </summary>
        public string Name { get; private set; }

        /// <summary> 引用计数 </summary>
        public int RefCount { get; private set; }

        /// <summary> 异步加载请求 </summary>
        public AssetBundleCreateRequest Request { get; set; }

        /// <summary> 加载到的ab </summary>
        public AssetBundle AssetBundle { get; set; }

        /// <summary> 依赖计数 </summary>
        public int DependLoadingCount;
   
        /// <summary> 依赖项 </summary>
        public List<string> Depends { get; private set; }

        private readonly List<AssetBundleLoadCallBack> _callFunList; //回调函数

        public AssetBundleObject(string name)
        {
            Name = name;
            Depends = new List<string>();
            _callFunList = new List<AssetBundleLoadCallBack>();
        }

        public void RegisterCallback(AssetBundleLoadCallBack callBack)
        {
            _callFunList.Add(callBack);
        }

        public void PlayCallback()
        {
            foreach (var callBack in _callFunList)
            {
                callBack(AssetBundle);
            }
            _callFunList.Clear();
        }
        
        public void AddRefCount()
        {
            RefCount++;
        }

        public void ReduceRefCount()
        {
            RefCount--;
        }

        // 强制异步转同步
        public void ForceSync()
        {
            if (Request != null)
            {
                AssetBundle = Request.assetBundle;
            }
        }
    }
}