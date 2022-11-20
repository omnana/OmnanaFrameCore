using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Assertions;

namespace FrameCore.Runtime
{
    /// <summary>
    /// 断点续传，要做的事如下：
    ///（1）网络请求异常处理——断网、请求失败、请求超时、网络波动、下载到一半等问题
    ///（2）文件读写异常处理——文件读失败、文件写失败、文件写一半等问题
    ///（3）游戏进程异常处理——下载到一半、文件写一半、玩家退出等问题
    ///（4）重启现场恢复处理——可恢复性，继续下载
    ///（5）文件的下载正确性——文件长度、文件校验，文件可读性
    ///（6）文件线程高效下载——多线程，异步回调文件
    /// </summary>
    public class DownloadModule : IDownloadModule, IUpdater
    {
        private readonly object _lock = new object();
        private readonly Queue<DownloadTask> _readyQueue;
        private readonly List<DownloadUnit> _completeList;
        private readonly List<DownloadTask> _errorList;
        private readonly List<Thread> _removeList;
        private readonly Dictionary<Thread, DownloadTask> _runTaskDict;

        public DownloadModule()
        {
            _readyQueue = new Queue<DownloadTask>();
            _completeList = new List<DownloadUnit>();
            _errorList = new List<DownloadTask>();
            _removeList = new List<Thread>();
            _runTaskDict = new Dictionary<Thread, DownloadTask>();
        }

        public void DownloadAsync(DownloadUnit unit)
        {
            Assert.IsTrue(unit != null, "DownloadUnit 不能为空！！");
            var dt = new DownloadTask(unit);
            lock (_lock)
            {
                _readyQueue.Enqueue(dt);
            }
        }
        
        public bool DownloadSync(DownloadUnit unit)
        {
            Assert.IsTrue(unit != null, "DownloadUnit 不能为空！！");
            var task = new DownloadTask(unit);
            try // 同步下载尝试三次
            {
                task.Run();
                if (task.State == DownloadTaskState.Complete) return true;
                task.Run();
                if (task.State == DownloadTaskState.Complete) return true;
                task.Run();
                if (task.State == DownloadTaskState.Complete) return true;
            }
            catch (Exception ex)
            {
                Debug.Log($"Error DownloadSync {task.State} {task.Unit.Name} {ex.Message}");
            }

            return false;
        }

        private void OnTaskLoop()
        {
            while (true)
            {
                DownloadTask dt = null;
                lock (_lock)
                {
                    if (_readyQueue.Count > 0)
                    {
                        dt = _readyQueue.Dequeue();
                        _runTaskDict[Thread.CurrentThread] = dt;
                        if (dt != null && dt.Unit.IsStop) // 已经销毁，不提取运行，直接删除
                        {
                            _runTaskDict[Thread.CurrentThread] = null;
                            continue;
                        }
                    }
                }

                // 已经没有需要下载的了
                if (dt == null)
                    break;

                dt.Run();
                lock (_lock)
                {
                    if (dt.State == DownloadTaskState.Complete)
                    {
                        _completeList.Add(dt.Unit);
                        _runTaskDict[Thread.CurrentThread] = null;
                    }
                    else if (dt.State == DownloadTaskState.Error)
                    {
                        _readyQueue.Enqueue(dt);
                        // 防止失败频繁回调，只在特定次数回调
                        if (dt.IsNeedErrorCall())
                        {
                            _errorList.Add(dt);
                        }

                        break;
                    }
                    else
                    {
                        Debug.Log("Error DownloadMacState " + dt.State + " " + dt.Unit.Name);
                        break;
                    }
                }
            }
        }

        public void StopDownload(DownloadUnit unit)
        {
            lock (_lock)
            {
                unit.IsStop = true;
            }
        }

        public void StopAll()
        {
            lock (_lock)
            {
                foreach (var task in _readyQueue)
                {
                    if (task != null) task.Unit.IsStop = true;
                }

                foreach (var kv in _runTaskDict)
                {
                    if (kv.Value != null) kv.Value.Unit.IsStop = true;
                }

                foreach (var unit in _completeList)
                {
                    if (unit != null) unit.IsStop = true;
                }
            }
        }

        private void UpdateComplete()
        {
            if (_completeList.Count == 0)
                return;

            DownloadUnit[] completes;
            lock (_lock)
            {
                completes = _completeList.ToArray();
                _completeList.Clear();
            }

            for (int i = 0, count = completes.Length; i < count; i++)
            {
                var info = completes[i];
                if (info.IsStop)
                    continue;

                info.IsStop = true;
                try
                {
                    if (info.Size != 0)
                        info.ProgressHandle?.Invoke(info, info.Size, info.Size);
                    info.CompleteHandle?.Invoke(info);
                }
                catch (Exception ex)
                {
                    Debug.LogError("UpdateComplete Error : " + ex.Message);
                }
            }
        }

        private void UpdateProgress()
        {
            if (_runTaskDict.Count == 0)
                return;

            var runTasks = new List<DownloadTask>();
            lock (_lock)
            {
                foreach (var mac in _runTaskDict.Values)
                {
                    if (mac != null)
                        runTasks.Add(mac);
                }
            }

            foreach (var task in runTasks)
            {
                var unit = task.Unit;
                if (unit.IsStop || unit.ProgressHandle == null) //已经销毁，不进行回调
                    continue;

                try
                {
                    if (task.AllSize != 0)
                        unit.ProgressHandle(unit, task.CurSize, task.AllSize);
                }
                catch (Exception ex)
                {
                    Debug.LogError("UpdateProgress Error : " + ex.Message);
                }
            }
        }

        // 打印收集错误信息
        private void UpdateError()
        {
            if (_errorList.Count == 0)
                return;

            DownloadTask[] errorTask;
            lock (_lock)
            {
                errorTask = _errorList.ToArray();
                _errorList.Clear();
            }

            foreach (var task in errorTask)
            {
                var info = task.Unit;
                if (info.IsStop || info.ErrorHandle == null)
                    continue;

                info.ErrorHandle(info, task.Error);
                task.Reset();
            }
        }

        private void UpdateTask()
        {
            if (_readyQueue.Count == 0 && _runTaskDict.Count == 0)
                return;

            // 关闭卡死的线程
            lock (_lock)
            {
                _removeList.Clear();
                foreach (var kv in _runTaskDict)
                {
                    // 已经在运行的跳过
                    if (kv.Key.IsAlive && !kv.Value.Unit.IsStop)
                        continue;
                    
                    _removeList.Add(kv.Key);
                    if (kv.Value != null)
                        _readyQueue.Enqueue(kv.Value);
                }

                foreach (var t in _removeList)
                {
                    _runTaskDict.Remove(t);
                    t.Abort();
                }
            }

            if (!NetHelper.IsNetConnect() || _runTaskDict.Count >= DownloadDefine.MAX_TASK_COUNT || _readyQueue.Count <= 0)
                return;

            var task = new Thread(OnTaskLoop);
            lock (_lock)
            {
                _runTaskDict.Add(task, null);
            }
            task.Start();
        }

        public void Update()
        {
            UpdateComplete();
            UpdateProgress();
            UpdateError();
            UpdateTask();
        }
    }
}
