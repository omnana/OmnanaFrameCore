using System;
using System.Collections.Generic;

namespace FrameCore.Runtime
{
    public abstract class BaseNode
    {
        public NodeKey Key { get; set; }
        public BaseNodeVO VO { get; set; }
        private readonly List<IController> _controllers;
        private Action _onUpdateDelegate;
        private Action _onLateUpdateDelegate;

        protected BaseNode()
        {
            _controllers = new List<IController>();
        }

        public void InitStaticNodeKey(NodeKey key)
        {
        }

        #region 生命周期

        public void Init()
        {
            foreach (var controller in _controllers)
            {
                controller.Init();
            }
        }

        public void Show()
        {
            foreach (var controller in _controllers)
            {
                controller.IsActive = true;
                controller.Show();
            }
        }

        public void Refresh(params object[] args)
        {
            foreach (var controller in _controllers)
            {
                controller.Refresh(args);
            }
        }

        public void Update()
        {
            _onUpdateDelegate?.Invoke();
        }

        public void LateUpdate()
        {
            _onLateUpdateDelegate?.Invoke();
        }

        public void Hide()
        {
            foreach (var controller in _controllers)
            {
                controller.IsActive = false;
                controller.Hide();
            }
        }

        public void Stop()
        {
            VO.Destroy();
            foreach (var controller in _controllers)
            {
                controller.Stop();
                _onUpdateDelegate -= controller.Update;
                _onLateUpdateDelegate -= controller.LateUpdate;
                RemoveController(controller);
            }

            _controllers.Clear();
        }

        #endregion

        #region Controller

        public void AddController<T, TM>(NodeObject obj) where T : IController where TM : BaseNodeVO
        {
            var controller = Activator.CreateInstance<T>();
            if (VO == null)
            {
                VO = Activator.CreateInstance<TM>();
                InjectHelper.Reflect(VO);
                VO.SetObj(obj);
                VO.Init();
            }

            controller.SetVO((TM) VO);
            _onUpdateDelegate += controller.Update;
            _onLateUpdateDelegate += controller.LateUpdate;
            InjectHelper.Reflect(controller);
            _controllers.Add(controller);
            AddController(controller);
        }

        protected virtual void AddController(IController controller)
        {
        }

        protected virtual void RemoveController(IController controller)
        {
        }

        public void AddController<T>(NodeObject obj) where T : BaseEmptyNodeController
        {
            var controller = Activator.CreateInstance<T>();
            _onUpdateDelegate += controller.Update;
            _onLateUpdateDelegate += controller.LateUpdate;
            InjectHelper.Reflect(controller);
            _controllers.Add(controller);
            AddController(controller);
        }
        
        #endregion
    }
}
