using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace FrameCore.Runtime
{
    /// <summary>
    /// 通用按钮
    /// </summary>
    public class UIPressButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
    {
        [HideInInspector]
        public Action OnPressDown;
        [HideInInspector]
        public Action OnPressUp;
        [HideInInspector]
        public Action OnPressExist;
  

        public void OnPointerDown(PointerEventData eventData)
        {
            OnPressDown?.Invoke();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            OnPressUp?.Invoke();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            OnPressExist?.Invoke();
        }
    }
}
