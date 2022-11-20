namespace FrameCore.Runtime
{
    public enum DownloadTaskState
    {
        None,
        ReadSize,
        Download,
        Md5,
        Complete,
        Error,
    }
}
