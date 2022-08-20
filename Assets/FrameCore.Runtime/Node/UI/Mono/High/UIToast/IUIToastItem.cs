namespace FrameCore.Runtime
{
    public interface IUIToastItem
    {
        void Show(string message, float lifeTime, System.Action<UIToastItem> onComplete);
    }
}
