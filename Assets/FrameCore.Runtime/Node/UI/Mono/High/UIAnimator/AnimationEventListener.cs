using System;
using UnityEngine;

namespace FrameCore.Runtime
{
    public class AnimationEventListener : MonoBehaviour
    {
        public Action<int> OnAniEvent { get; set; }

        public void OnEvent(int index)
        {
            OnAniEvent?.Invoke(index);
        }
    }
}
