using System;
using System.Collections.Generic;

namespace FrameCore.Runtime
{
    public class ObserverModule : IObserverModule
    {
        private readonly Dictionary<ValueType, Action<object[]>> _stock;
        public ObserverModule()
        {
            _stock = new Dictionary<ValueType, Action<object[]>>();
        }

        public void Add<T>(T eventEnum, Action<object[]> callBack) where T : struct
        {
            if (!_stock.ContainsKey(eventEnum))
            {
                _stock.Add(eventEnum, callBack);
                return;
            }

            var list = _stock[eventEnum].GetInvocationList();
            foreach (Action<object[]> method in list)
            {
                if (method != callBack)
                    continue;

                FrameDebugger.LogWarning("Method does exist：" + method.Method.Name);
                return;
            }

            _stock[eventEnum] += callBack;
        }

        public void Remove<T>(T eventEnum, Action<object[]> callBack) where T : struct
        {
            if (callBack == null)
                return;

            if (!_stock.ContainsKey(eventEnum))
            {
                FrameDebugger.LogWarning("Method doesn't exist：" + callBack);
                return;
            }

            if ((_stock[eventEnum] -= callBack) != null)
                return;

            _stock.Remove(eventEnum);
        }

        public void Dispatch<T>(T eventEnum, params object[] args) where T : struct
        {
            if (!_stock.ContainsKey(eventEnum))
                return;

            if (_stock[eventEnum] == null)
            {
                _stock.Remove(eventEnum);
                return;
            }

            _stock[eventEnum](args);
        }

        [Obsolete("It is dangerous to do this")]
        public void Clear()
        {
            _stock.Clear();
        }
    }
}