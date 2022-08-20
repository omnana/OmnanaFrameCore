using UnityEngine;

namespace FrameCore.Runtime
{
    public interface IUIToast
    {
        void ShowMessage(Transform trans, string message);
        void ShowMessage(Vector2 anchoredPosition, string message);
        void Clear();
    }
}
