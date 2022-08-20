using UnityEngine;

namespace FrameCore.Runtime
{
    public interface IEditorAssetModule
    {
        void SetProduct();
        bool IsFileExist(string assetName);
        T LoadSync<T>(string assetName) where T : Object;
        void Unload(Object asset);
    }
}
