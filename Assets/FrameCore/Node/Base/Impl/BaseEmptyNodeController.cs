namespace FrameCore.Runtime
{
    public class BaseEmptyNodeController : IController
    {
        public int Id { get; private set; }
        public bool IsActive { get; set; }

        public void SetVO(BaseNodeVO vo)
        {
        }

        public void Init() => OnInit();
        public void Show() => OnShow();
        public void Refresh(params object[] args) => OnRefresh(args);
        public void Update() => OnUpdate();
        public void LateUpdate() => OnLateUpdate();
        public void Hide() => OnHide();
        public void Stop() => OnStop();

        protected virtual void OnInit()
        {
        }

        protected virtual void OnShow()
        {
        }

        protected virtual void OnRefresh(params object[] args)
        {
        }

        protected virtual void OnUpdate()
        {
        }

        protected virtual void OnLateUpdate()
        {
        }

        protected virtual void OnHide()
        {
        }

        protected virtual void OnStop()
        {
        }
    }
}