using System;

namespace FrameCore.Runtime
{
    public abstract class AStateBase
    {
        private Action<Type, object[]> callback;
        public abstract void Init();
        public abstract void OnEnter(params object[] args);
        public abstract void OnUpdate();
        public abstract void OnExit();

        public void InjectCallBack(Action<Type, object[]> callBack)
        {
            this.callback = callBack;
        }

        public void Change<T>(params object[] args) where T : AStateBase
        {
            callback(typeof(T), args);
        }
    }
}