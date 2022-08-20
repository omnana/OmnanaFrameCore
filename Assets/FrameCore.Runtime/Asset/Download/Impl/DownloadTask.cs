using System;
using System.IO;
using System.Net;
using FrameCore.Utility;
using UnityEngine;

namespace FrameCore.Runtime
{
    // 下载任务
    public class DownloadTask
    {
        public DownloadUnit Unit { get; private set; }
        public int CurSize { get; private set; }
        public int AllSize { get; private set; }
        public int TryCount { get; private set; }
        public DownloadTaskState State { get; private set; } = DownloadTaskState.None;
        public string Error { get; private set; }
        public bool IsWriting { get; private set; }
        
        public DownloadTask(DownloadUnit unit)
        {
            Unit = unit;
        }

        public void Run()
        {
            TryCount++;
            
            if (!ReadSize())
                return;

            if (!Download())
                return;

            if (!CheckMd5())
            {
                if (!Download())
                    return;

                if (!CheckMd5()) // 再做一次，还是失败直接返回
                    return;
            }

            State = DownloadTaskState.Complete;
        }

        public void Reset()
        {
            State = DownloadTaskState.None;
            Error = string.Empty;
            TryCount = 0;
        }

        public void Stop()
        {
            
        }

        // 读取文件长度
        private bool ReadSize()
        {
            State = DownloadTaskState.ReadSize;
            if (Unit.Size <= 0)
            {
                HttpWebRequest request = null;
                WebResponse response = null;
                try
                {
                    request = CreateWebRequest(Unit.DownUrl);
                    request.Method = "HEAD";
                    response = request.GetResponse();
                    Unit.Size = (int) response.ContentLength;
                }
                catch (Exception e)
                {
                    Error = e.Message;
                    State = DownloadTaskState.Error;
                    Debug.LogError($"webRequest error : {e.Message}");
                }
                finally
                {
                    response?.Close();
                    request?.Abort();
                }
            }

            CurSize = 0;
            AllSize = Unit.Size;
            return true;
        }

        // 下载
        private bool Download()
        {
            State = DownloadTaskState.Download;

            // 文件已经存在
            if (File.Exists(Unit.SavePath))
            {
                CurSize = Unit.Size;
                return true;
            }

            long startPos = 0;
            var tempFile = $"{Unit.SavePath}.temp";
            FileStream fs = null;
            try
            {
                if (File.Exists(tempFile)) // 已经有部分下载到本地
                {
                    fs = File.OpenWrite(tempFile);
                    startPos = fs.Length;
                    fs.Seek(startPos, SeekOrigin.Current);
                    if (startPos == Unit.Size)
                    {
                        fs.Flush();
                        fs.Close();
                        fs = null;
                        FileUtility.SaveFile(Unit.SavePath, tempFile);
                    }
                }
                else
                {
                    // 重新创建
                    var direName = Path.GetDirectoryName(tempFile);
                    if (!Directory.Exists(direName))
                        Directory.CreateDirectory(direName);
                    fs = new FileStream(tempFile, FileMode.Create);
                }
            }
            catch (Exception e)
            {
                fs?.Close();
                Error = e.Message;
                State = DownloadTaskState.Error;
                Debug.LogError($"Error : {e.Message}");
                return false;
            }

            // 断点续传
            HttpWebRequest rq = null;
            HttpWebResponse rp = null;
            Stream ns = null;
            try
            {
                rq = CreateWebRequest(Unit.DownUrl);
                // 设置Range值，断点续传
                if (startPos > 0)
                    rq.AddRange((int) startPos);
                rp = (HttpWebResponse) rq.GetResponse();
                ns = rp.GetResponseStream();
                ns.ReadTimeout = DownloadDefine.TimeOutWait;
                var totalSize = rp.ContentLength;
                var curSize = startPos;
                if (curSize == totalSize)
                {
                    fs.Flush();
                    fs.Close();
                    fs = null;
                    CurSize = (int) startPos;
                    FileUtility.SaveFile(Unit.SavePath, tempFile);
                }
                else
                {
                    var bytes = new byte[DownloadDefine.DownloadLen];
                    var readSize = ns.Read(bytes, 0, bytes.Length); // 读取第一份数据
                    while (readSize > 0)
                    {
                        if (CheckStop()) break;
                        fs.Write(bytes, 0, readSize); // 将下载到的数据写入临时文件
                        curSize += readSize;
                        CurSize = (int) curSize;
                        if (curSize == totalSize)
                        {
                            fs.Flush();
                            fs.Close();
                            fs = null;
                            FileUtility.SaveFile(Unit.SavePath, tempFile);
                        }

                        if (CheckStop()) break;
                        readSize = ns.Read(bytes, 0, DownloadDefine.DownloadLen);
                    }
                }
            }
            catch (Exception e)
            {
                //下载失败，删除临时文件
                if (fs != null)
                {
                    fs.Flush();
                    fs.Close();
                    fs = null;
                }

                if (File.Exists(tempFile))
                    File.Delete(tempFile);

                if (File.Exists(Unit.SavePath))
                    File.Delete(Unit.SavePath);

                State = DownloadTaskState.Error;
                Error = e.Message;
                Debug.LogError($"download error : {e.StackTrace}");
            }
            finally
            {
                fs?.Flush();
                fs?.Close();
                ns?.Close();
                rq?.Abort();
                rp?.Close();
            }

            return State != DownloadTaskState.Error && State != DownloadTaskState.None;
        }

        private bool CheckStop()
        {
            if (!Unit.IsStop) 
                return false;
            
            State = DownloadTaskState.None;
            return true;

        }

        // 检查md5
        private bool CheckMd5()
        {
            State = DownloadTaskState.Md5;
            if (string.IsNullOrEmpty(Unit.Md5)) //不做校验，默认成功
                return true;

            var md5 = Md5Helper.GetMd5HashFromFile(Unit.SavePath, DownloadDefine.DownloadLen);
            if (md5 == Unit.Md5)
                return true;

            File.Delete(Unit.SavePath);
            State = DownloadTaskState.Error;
            Error = "Check MD5 Error ";
            return false;
        }

        /// <summary>
        /// 防止失败频繁回调，只在特定次数回调
        /// </summary>
        /// <returns></returns>
        public bool IsNeedErrorCall()
        {
            return TryCount == 3 || TryCount == 10 || TryCount == 100;
        }

        private HttpWebRequest CreateWebRequest(string url)
        {
            HttpWebRequest rq = null;
            try
            {
                rq = WebRequest.Create(url) as HttpWebRequest;
                rq.Timeout = DownloadDefine.TimeOut;
                rq.ReadWriteTimeout = DownloadDefine.TimeOutWait;
            }
            catch (Exception e)
            {
                Debug.LogError($"webRequest error : {e.Message}");
            }
            
            return rq;
        }
    }
}
