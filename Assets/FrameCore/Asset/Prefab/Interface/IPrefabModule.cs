using UnityEngine;

namespace FrameCore.Runtime
{
    public interface IPrefabModule
    {
        GameObject LoadSync(string assetName, Transform parent = null, bool instantiateInWorldSpace = false);
        void LoadAsync(string assetName, PrefabLoadCallback callFun, Transform parent = null, bool instantiateInWorldSpace = false);
        void RemoveCallback(string assetName, PrefabLoadCallback callFun);
        void AddAssetRef(string assetName, GameObject gameObject);
        void Destroy(GameObject obj);
    }
}
