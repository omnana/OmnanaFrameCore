using System.Collections.Generic;

namespace FrameCore.Editor
{
    public class AbNode
    {
        public string Name;
        public string Path;
        public bool IsRoot;
        public bool IsCombine; // 已经被绑走标识
        public readonly List<string> Combinelist = new List<string>();
        public readonly Dictionary<string, AbNode> Dependences = new Dictionary<string, AbNode>(); // 依赖项
        public readonly Dictionary<string, AbNode> BeDependences = new Dictionary<string, AbNode>(); // 被依赖项
    }
}
