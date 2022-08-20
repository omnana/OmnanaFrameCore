using UnityEngine;

namespace FrameCore.Runtime
{
    public interface IGoGetter
    {
        T GetGo<T>(string key) where T : Object;
    }
}
