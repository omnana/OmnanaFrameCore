using System;
using FrameCore.Util;

namespace FrameCore.Runtime
{
    public interface IObserverModule
    {
        void Add<T>(T eventEnum, Action<object[]> callBack) where T : struct;
        void Remove<T>(T eventEnum, Action<object[]> callBack) where T : struct;
        void Dispatch<T>(T eventEnum, params object[] args) where T : struct;
        void Clear();
    }
}