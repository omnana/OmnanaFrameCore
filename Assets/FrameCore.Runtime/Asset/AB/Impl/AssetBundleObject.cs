using System.Collections.Generic;
using UnityEngine;

namespace FrameCore.Runtime
{
    public delegate void AssetBundleLoadCallBack(AssetBundle ab);
    public sealed class AssetBundleObject
    {
        public AssetBundleState State { get; set; } = AssetBundleState.None;
        public string Name { get; private set; } // hash标识符

        public int RefCount { get; private set; } // 引用计数

        public AssetBundleCreateRequest Request { get; set; } // 异步加载请求

        public AssetBundle AssetBundle { get; set; }// 加载到的ab

        public int DependLoadingCount; // 依赖计数
   
        public List<AssetBundleObject> Depends; // 依赖项

        private readonly List<AssetBundleLoadCallBack> _callFunList; //回调函数

        public AssetBundleObject(string name)
        {
            Name = name;
            Depends = new List<AssetBundleObject>();
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

        // 遍历整个关系树
        public void DoDependsRef()
        {
            RefCount++;
            foreach (var dp in Depends)
            {
                dp.DoDependsRef();
            }
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

            foreach (var dp in Depends)
            {
                dp.ForceSync();
            }
        }
    }
}
