namespace FrameCore.Runtime
{
    public class DownloadDefine
    {
        /// <summary>
        /// 一次读取的长度 16kb
        /// </summary>
        public const int DownloadLen = 16 * 1024;
    
        /// <summary>
        /// 一次读取md5长度 16 kb
        /// </summary>
        public const int Md5ReadLen = 16 * 1024;
    
        /// <summary>
        /// 超时等待时间
        /// </summary>
        public const int TimeOut = 2 * 1000;

        /// <summary>
        /// 超时等待时间
        /// </summary>
        public const int TimeOutWait = 5 * 1000;
        
        /// <summary>
        /// 允许的当前执行的下载任务最大数
        /// </summary>
        public const int MAX_TASK_COUNT = 20;
    }
}
