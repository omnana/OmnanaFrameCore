namespace FrameCore.Runtime
{
    public interface IDownloadModule
    {
        void DownloadAsync(DownloadUnit unit);
        bool DownloadSync(DownloadUnit unit);
        void StopDownload(DownloadUnit unit);
        void StopAll();
    }
}
