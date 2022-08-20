using UnityEngine;

namespace FrameCore.Runtime
{
    public interface IResourceModule
    {
        bool IsFileExist(string assetName);
        ResourceRequest LoadAsync(string assetName);
        T LoadSync<T>(string assetName) where T : Object;
        void Unload(Object asset);
    }
}
