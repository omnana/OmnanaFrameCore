namespace FrameCore.Runtime
{
    public interface IAssetBundleModule
    {
        void LoadManifest();
        bool IsFileExist(string assetName);
        AssetBundleObject LoadSync(string assetName);
        void  LoadAsync(string abName, AssetBundleLoadCallBack callFunc);
        void Unload(string abName);
    }
}
