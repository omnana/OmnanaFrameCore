using System;
using System.Collections.Generic;

namespace FrameCore.Runtime
{
    public class ObserverModule : IObserverModule
    {
        private Dictionary<ValueType, Action<object[]>> stock;
        public ObserverModule()
        {
            stock = new Dictionary<ValueType, Action<object[]>>();
        }

        public void Add<T>(T eventEnum, Action<object[]> callBack) where T : struct
        {
            if (!stock.ContainsKey(eventEnum))
            {
                stock.Add(eventEnum, callBack);
                return;
            }

            foreach (Action<object[]> method in stock[eventEnum].GetInvocationList())
            {
                if (method != callBack)
                    continue;

                FrameDebugger.LogWarning("Method does exist：" + method.Method.Name);
                return;
            }

            stock[eventEnum] += callBack;
        }

        public void Remove<T>(T eventEnum, Action<object[]> callBack) where T : struct
        {
            if (callBack == null)
                return;

            if (!stock.ContainsKey(eventEnum))
            {
                FrameDebugger.LogWarning("Method doesn't exist：" + callBack.ToString());
                return;
            }

            if ((stock[eventEnum] -= callBack) != null)
                return;

            stock.Remove(eventEnum);
        }

        public void Dispatch<T>(T eventEnum, params object[] args) where T : struct
        {
            if (!stock.ContainsKey(eventEnum))
                return;

            if (stock[eventEnum] == null)
            {
                stock.Remove(eventEnum);
                return;
            }

            stock[eventEnum](args);
        }

        [Obsolete("It is dangerous to do this")]
        public void Clear()
        {
            stock.Clear();
        }
    }
}