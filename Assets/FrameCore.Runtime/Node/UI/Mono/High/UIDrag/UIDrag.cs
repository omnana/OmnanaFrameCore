using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace FrameCore.Runtime
{
    public class OnDragStart : UnityEvent<GameObject>
    {
    }

    public class OnDrag : UnityEvent<GameObject>
    {
    }

    public class OnDragEnd : UnityEvent<GameObject>
    {
    }

    public interface IUIDrag
    {
        bool EnableDrag { get; set; }
        OnDragStart OnDragStart { get; set; }
        OnDrag OnDrag { get; set; }
        OnDragEnd OnDragEnd { get; set; }
    }

    public class UIDrag : MonoBehaviour, IUIDrag, IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        public OnDragStart OnDragStart { get; set; } = new OnDragStart();
        public OnDrag OnDrag { get; set; } = new OnDrag();
        public OnDragEnd OnDragEnd { get; set; } = new OnDragEnd();

        private Vector3 _mousePos;

        private RectTransform _rect;

        public bool EnableDrag { get; set; } = true;

        private void Awake()
        {
            _rect = GetComponent<RectTransform>();
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (!EnableDrag)
                return;

            _mousePos = Input.mousePosition;
            OnDragStart?.Invoke(this.gameObject);
        }

        void IDragHandler.OnDrag(PointerEventData eventData)
        {
            if (!EnableDrag)
                return;

            _rect.anchoredPosition += (Vector2) (Input.mousePosition - _mousePos);
            _mousePos = Input.mousePosition;
            OnDrag?.Invoke(gameObject);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (!EnableDrag)
                return;

            OnDragEnd?.Invoke(this.gameObject);
        }
    }
}
