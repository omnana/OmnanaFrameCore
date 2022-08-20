using UnityEngine;

namespace FrameCore.Runtime
{
    public interface IAssetModule
    {
        void Load();
        bool IsFileExist(string assetName);
        T LoadSync<T>(string assetName) where T : Object;
        void LoadAsync(string assetName, AssetsLoadCallback callFun);
        void PreLoad(string assetName, bool isWeak = true);
        string GetAssetAbName(string assetName);
        void Unload(Object obj);
    }
}
