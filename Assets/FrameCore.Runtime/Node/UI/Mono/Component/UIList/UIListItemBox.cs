using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace FrameCore.Runtime
{
    public class UIListItemBox : MonoBehaviour, IPointerClickHandler
    {
        private ContentSizeFitter _contentSizeFitter;
        public int Index { get; set; }
        public OnItemClickEvent OnItemClick { get; set; }

        private RectTransform _rectTransform;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            _contentSizeFitter = GetComponent<ContentSizeFitter>();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            OnItemClick?.Invoke(Index, gameObject);
        }

        public void ForceLayout()
        {
            if (_contentSizeFitter != null)
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(_rectTransform);
            }
        }
    }
}