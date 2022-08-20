using UnityEngine;

namespace FrameCore.Runtime
{
    public class UIToast : MonoBehaviour, IUIToast
    {
        [SerializeField] public float LifeTime;
        [SerializeField] public GameObject _item;
        private UIItemPool _itemPool;
        
        private void Awake()
        {
            _itemPool = new UIItemPool();
        }

        public void ShowMessage(Transform trans, string message)
        {
            var item = _itemPool.New(_item, transform);
            var toastItem = item.GetComponent<UIToastItem>();
            toastItem.transform.position = trans.position;
            toastItem.gameObject.SetActive(true);
            toastItem.Show(message, LifeTime, uiToastItem =>
            {
                toastItem.gameObject.SetActive(false);
                _itemPool.Recycle(toastItem.gameObject);
            });
        }

        public void ShowMessage(Vector2 anchoredPosition, string message)
        {
            var item = _itemPool.New(_item, transform);
            var toastItem = item.GetComponent<UIToastItem>();
            toastItem.GetComponent<RectTransform>().anchoredPosition = anchoredPosition;
            toastItem.gameObject.SetActive(true);
            toastItem.Show(message, LifeTime, uiToastItem =>
            {
                toastItem.gameObject.SetActive(false);
                _itemPool.Recycle(toastItem.gameObject);
            });
        }

        public void Clear()
        {
            _itemPool.Clear();
        }
    }
}
