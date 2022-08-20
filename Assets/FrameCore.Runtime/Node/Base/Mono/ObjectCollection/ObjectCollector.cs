using System;
using System.Collections.Generic;
using UnityEngine;

namespace FrameCore.Runtime
{
    [Serializable]
    public class ObjectCollector
    {
#if UNITY_EDITOR
        public Type collectorType = typeof(UnityEngine.Object);
        public Type objectType = typeof(UnityEngine.Object);
        public Type[] blackTypes = null;
        public bool isGenerate = false;
#endif

        [SerializeField]
        public List<string> keys = new List<string>();

        [SerializeField]
        public List<UnityEngine.Object> values = new List<UnityEngine.Object>();

        private Dictionary<string, UnityEngine.Object> componentsDict;
        private bool dictCached = false;

        public UnityEngine.Object this[string key]
        {
            get
            {
                if (!dictCached) CacheDict();
                if (componentsDict.TryGetValue(key, out UnityEngine.Object value))
                    return value;
                Debug.LogError($"Can't find the key: {key}, Please Check it");
                return null;
            }
        }

        public List<string> Keys => keys;

        public List<UnityEngine.Object> Objects => values;

        private void CacheDict()
        {
            dictCached = true;

            componentsDict = new Dictionary<string, UnityEngine.Object>(keys.Count);
            for (int i = 0, length = keys.Count; i < length; i++)
            {
                componentsDict[keys[i]] = values[i];
            }
        }
    }
}
