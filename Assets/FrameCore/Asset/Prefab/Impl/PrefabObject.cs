using System.Collections.Generic;
using UnityEngine;

namespace FrameCore.Runtime
{
    public delegate void PrefabLoadCallback(string assetName, GameObject obj);
    
    public sealed class PrefabObject
    {
        public string Name { get; set; }

        public int LockCallbackCount { get; set; } //记录回调当前数量，保证异步是下一帧回调

        public List<PrefabLoadCallback> CallbackList = new List<PrefabLoadCallback>();

        public Object Asset { get; set; }

        public int RefCount { get; set; } = 1;

        public HashSet<int> GoInstanceIdSet = new HashSet<int>(); //实例化的GameObject引用列表

        public Transform parent;

        public bool instantiateInWorldSpace;
    }
}