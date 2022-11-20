namespace FrameCore.Runtime
{
    public interface IController
    {
        int Id { get; }
        bool IsActive { get; set; }
        void SetVO(BaseNodeVO vo);
        void Init();
        void Show();
        void Refresh(params object[] args);
        void Update();
        void LateUpdate();
        void Hide();
        void Stop();
    }
}