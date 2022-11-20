using System.Collections.Generic;

namespace FrameCore.Runtime
{
    public static class NodeKeyCollector
    {
        private static readonly Dictionary<string, NodeKey> KeyDic = new Dictionary<string, NodeKey>();

        public static void AddKey(string nodeName, NodeKey nodeKey)
        {
            KeyDic[nodeName] = nodeKey;
        }

        public static NodeKey GetKey(string key)
        {
            KeyDic.TryGetValue(key, out var keyValue);
            return keyValue;
        }
    }
}