using System.Collections.Generic;
using UnityEngine.Assertions;
using UnityEngine.UI;
using UnityEngine;
using System;

namespace FrameCore.Runtime
{
    /// <summary>
    /// 简单列表，需要循环复用再扩展
    /// </summary>
    public class UIList : MonoBehaviour, IUIList
    {
        [SerializeField] private GameObject _itemPrefab;
        [SerializeField] private RectTransform _content;

        public int Count
        {
            get => _dataCount;
            set
            {
                _dataCount = value;
                Assert.IsTrue(_dataCount >= 0, "Count必须大于或等于0！！");
                Refresh();
            }
        }

        private int _dataCount;

        public OnItemDrawEvent OnStart { get; set; } = new OnItemDrawEvent();
        public OnItemDrawEvent OnStop { get; set; } = new OnItemDrawEvent();
        public OnItemClickEvent OnItemClickEvent { get; set; } = new OnItemClickEvent();

        private UIItemPool _itemPool;

        private readonly List<GameObject> _items = new List<GameObject>();

        private LayoutGroup _layout;

        private void Awake()
        {
            _itemPool = new UIItemPool();
            _layout = _content.GetComponent<LayoutGroup>();
        }

        private void Refresh()
        {
            int itemCount = _items.Count;
            if (_dataCount < itemCount)
            {
                for (int i = itemCount - 1; i >= 0; i--)
                {
                    try
                    {
                        if (i >= _dataCount)
                        {
                            var item = _items[i];
                            OnStop?.Invoke(i, item);
                            item.gameObject.SetActive(false);
                            _itemPool.Recycle(item);
                            _items.RemoveAt(i);
                        }
                        else
                        {
                            OnStart?.Invoke(i, _items[i]);
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"UIList OnStop error : {e.StackTrace}");
                    }
                }
            }
            else
            {
                for (int i = 0; i < _dataCount; i++)
                {
                    try
                    {
                        if (i < itemCount)
                        {
                            OnStart?.Invoke(i, _items[i]);
                        }
                        else
                        {
                            var item = _itemPool.New(_itemPrefab, _content);
                            item.name = $"{_itemPrefab.name}_{i}";
                            var itemBox = item.GetComponent<UIListItemBox>();
                            if (itemBox == null)
                                itemBox = item.AddComponent<UIListItemBox>();
                            itemBox.Index = i;
                            itemBox.OnItemClick = OnItemClickEvent;
                            OnStart?.Invoke(i, item);
                            item.SetActive(true);
                            itemBox.ForceLayout();
                            _items.Add(item);
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"UIList OnStart error : {e.StackTrace}");
                    }
                }
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate(_content);
        }

        public GameObject GetItem(int index)
        {
            if (_dataCount <= index)
            {
                Debug.LogError($"index已经超过DataCount!!index:{index}");
                return null;
            }

            return _items[index].gameObject;
        }
    }
}