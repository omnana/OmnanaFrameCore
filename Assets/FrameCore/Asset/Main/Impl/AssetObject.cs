using System.Collections.Generic;
using UnityEngine;

namespace FrameCore.Runtime
{
    public delegate void AssetsLoadCallback(string name, Object obj);

    public sealed class AssetObject
    {
        public AssetObjStatus Status { get; set; } = AssetObjStatus.None;

        public string Name { get; set; }

        public int LockCallbackCount { get; set; } // 记录回调当前数量，保证异步是下一帧回调

        public List<AssetsLoadCallback> CallbackList = new List<AssetsLoadCallback>(); //回调函数

        public int InstanceID { get; set; } // asset的id

        public AsyncOperation Request { get; set; } // 异步请求，AssetBundleRequest或ResourceRequest

        public Object Asset { get; set; } // 加载的资源Asset

        public bool IsAbLoad { get; set; } // 标识是否是ab资源加载的

        /// <summary>
        /// 是弱引用标识，为true时，表示这个资源可以在没有引用时卸载，否则常驻内存。常驻内存是指引用计数为0也不卸载。
        /// </summary>
        public bool IsWeak { get; set; } = true; // 是否是弱引用，用于预加载和释放

        public int RefCount { get; private set; } = 0; // 引用计数

        public int UnloadTick { get; set; } // 卸载使用延迟卸载，UNLOAD_DELAY_TICK_BASE + _unloadList.Count

        public void AddRefCount()
        {
            RefCount++;
        }

        public void ReduceRefCount()
        {
            RefCount--;
        }

        public Object GetAsyncAsset()
        {
            return IsAbLoad ? (Request as AssetBundleRequest).asset : (Request as ResourceRequest).asset;
        }

        public void ForceSync()
        {
            Asset = GetAsyncAsset();
        }

        public void DoCallback()
        {
            if (CallbackList.Count == 0)
                return;

            var count = LockCallbackCount; //先提取count，保证回调中有加载需求不加载
            for (var i = 0; i < count; i++)
            {
                if (CallbackList[i] == null)
                    continue;

                AddRefCount(); //每次回调，引用计数+1
                try
                {
                    CallbackList[i](Name, Asset);
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"[AssetObject] error : {e.StackTrace}");
                }
            }

            CallbackList.RemoveRange(0, count);
        }
    }
}
