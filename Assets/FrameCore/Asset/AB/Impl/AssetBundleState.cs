namespace FrameCore.Runtime
{
    public enum AssetBundleState
    {
        None = 0,
        Ready = 1, // 准备加载
        Loading = 2, // 正在加载
        Loaded = 3, // 加载完成
        Unload = 4, // 待卸载
    }
}
