using System.Collections.Generic;
using UnityEngine;

namespace FrameCore.Editor
{
    public abstract class CodeFieldGetter
    {
        protected GameObject GO;

        public CodeFieldGetter(GameObject go)
        {
            GO = go;
        }

        public HashSet<string> GetFieldCode(string key, UnityEngine.Object value)
        {
            HashSet<string> set = new HashSet<string>();
            string typeStr = value.GetType().Name;
            string keyStr = key;
            string bindingStr = $"=> UIGetter.GetGo<{typeStr}>(\"{key}\")";
            set.Add($"public {typeStr} {keyStr} {bindingStr};");
            return set;
        }

        public HashSet<string> GetFieldCode1(string key, UnityEngine.Object value)
        {
            HashSet<string> set = new HashSet<string>();
            string typeStr = value.GetType().Name;
            string keyStr = key;
            set.Add($"private {typeStr} {GetKeyStr(keyStr)};");
            return set;
        }

        public HashSet<string> GetFieldCode2(string key, UnityEngine.Object value)
        {
            HashSet<string> set = new HashSet<string>();
            string typeStr = value.GetType().Name;
            string keyStr = key;
            string bindingStr = $"= obj.GetGo<{typeStr}>(\"{key}\")";
            set.Add($"{GetKeyStr(keyStr)} {bindingStr};");
            return set;
        }

        public HashSet<string> GetFieldCode3(string key, UnityEngine.Object value)
        {
            HashSet<string> set = new HashSet<string>();
            string typeStr = value.GetType().Name;
            string keyStr = key;
            string bindingStr = $"= Object.GetGo<{typeStr}>(\"{key}\")";
            set.Add($"{GetKeyStr(keyStr)} {bindingStr};");
            return set;
        }
        
        protected string GetKeyStr(string key)
        {
            string keyStr = string.Empty;
            if (key.Length == 1)
                keyStr = "_" + char.ToLower(key[0]);
            else
                keyStr = "_" + char.ToLower(key[0]) + key.Substring(1);
            return keyStr;
        }
    }
}
