using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace FrameCore.Runtime
{
    public class UIToastItem : MonoBehaviour, IUIToastItem
    {
        public Text Text;
        public float MoveDis = 10f;

        public void Show(string message, float lifeTime, Action<UIToastItem> onComplete)
        {
            var targetPos = Text.rectTransform.anchoredPosition;
            Text.text = message;
            // Text.ResetRichTextLineBreak();
            // Text.rectTransform.DOAnchorPos(targetPos + Vector2.up * MoveDis, lifeTime).onComplete = () =>
            // {
            //     onComplete?.Invoke(this);
            //     transform.position = targetPos;
            // };
        }
    }
}
