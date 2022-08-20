namespace FrameCore.Runtime
{
    public enum AssetObjStatus
    {
        None = 0,
        Loading = 1, // 正在加载
        Loaded = 2, // 加载完成
        Unload = 3, // 待卸载
    }
}