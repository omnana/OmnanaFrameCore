namespace FrameCore.Runtime
{
    public interface IAssetBundleModule
    {
        void LoadManifest();
        bool IsFileExist(string abName);
        AssetBundleObject LoadSync(string abName);
        void  LoadAsync(string abName, AssetBundleLoadCallBack callFunc);
        void Unload(string abName);
    }
}
