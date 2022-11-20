namespace FrameCore.Runtime
{
    public delegate void DownloadErrorHandle(DownloadUnit downloadUnit, string msg);

    public delegate void DownloadProgressHandle(DownloadUnit downloadUnit, int curSize, int allSize);

    public delegate void DownloadCompleteHandle(DownloadUnit downloadUnit);

    public class DownloadUnit
    {
        public string Name;
        public string DownUrl;
        public string SavePath;
        public int Size;
        public string Md5;
        public bool IsStop;
        public bool IsPause;
        public DownloadErrorHandle ErrorHandle;
        public DownloadProgressHandle ProgressHandle;
        public DownloadCompleteHandle CompleteHandle;
    }
}
