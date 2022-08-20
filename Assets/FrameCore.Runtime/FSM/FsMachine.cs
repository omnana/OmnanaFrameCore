using System;
using System.Collections.Generic;

namespace FrameCore.Runtime
{
    public class FsMachine : IUpdater
    {
        private readonly Dictionary<Type, AStateBase> _stateStock;
        private AStateBase _runningState;

        public FsMachine()
        {
            _stateStock = new Dictionary<Type, AStateBase>();
        }

        private void Change(Type type, params object[] args)
        {
            if (!_stateStock.TryGetValue(type, out AStateBase state))
            {
                FrameDebugger.LogWarning("State does not exist:" + type.Name);
                return;
            }

            if (_runningState == state)
                return;

            _runningState?.OnExit();
            _runningState = state;
            _runningState.OnEnter(args);
        }

        public void Add<T>() where T : AStateBase
        {
            Type type = typeof(T);

            if (_stateStock.ContainsKey(type))
            {
                FrameDebugger.LogWarning("State does exist:" + type.Name);
                return;
            }

            AStateBase state = (AStateBase) Activator.CreateInstance(type);
            state.InjectCallBack(Change);
            InjectHelper.Reflect(state);
            _stateStock.Add(type, state);
            state.Init();
        }

        public void Change<T>(params object[] args) where T : AStateBase
        {
            Change(typeof(T), args);
        }

        public bool IsStateOn<T>() where T : AStateBase
        {
            Type type = typeof(T);
            return _runningState != null && _runningState.GetType() == type;
        }

        public void Remove<T>() where T : AStateBase
        {
            Type type = typeof(T);
            if (!_stateStock.TryGetValue(type, out AStateBase state))
            {
                FrameDebugger.LogWarning("State does not exist:" + type.Name);
                return;
            }

            if (state == _runningState)
                _runningState = null;

            _stateStock.Remove(type);
        }

        public void Clear()
        {
            _runningState = null;
            _stateStock.Clear();
        }

        public void Update()
        {
            _runningState?.OnUpdate();
        }
    }
}