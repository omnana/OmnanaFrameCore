using System.Collections.Generic;
using UnityEngine;

namespace FrameCore.Runtime
{
    public class UIModule : IUIModule
    {
        private IPrefabModule PrefabModule => IocContainer.Resolve<IPrefabModule>();
        private readonly Dictionary<UIPanelKey, UINodeObject> _panelDic;
        private readonly Dictionary<UIPanelLayer, int> _layerSortIndexDic;

        public UIModule()
        {
            _panelDic = new Dictionary<UIPanelKey, UINodeObject>();
            _layerSortIndexDic = new Dictionary<UIPanelLayer, int>();
        }

        public void Open(UIPanelKey key)
        {
            if (!_panelDic.ContainsKey(key))
            {
                if (key.GetType() != typeof(UIPanelKey))
                {
                    FrameDebugger.LogError($"key:{key}; 这不是 UIPanelKey ！！！！！");
                    return;
                }

                var go = PrefabModule.LoadSync($"UI/{key}.prefab");
                if (go != null)
                {
                    var ui = go.GetComponent<UINodeObject>();
                    ui.GetComponent<Canvas>().sortingOrder = GetCurrentLayerSort(ui.UILayer);
                    var parent = GameObject.Find(ui.UILayer.ToDes()).transform;
                    go.transform.SetParent(parent, false);
                    go.transform.SetAsLastSibling();
                    go.SetActive(true);
                    _panelDic.Add(key, ui);
                }
            }

            _panelDic[key].Open();
        }

        public void Close(UIPanelKey key)
        {
            if (!_panelDic.ContainsKey(key))
                return;

            var ui = _panelDic[key];
            ui.SetActive(false);
        }

        public void Destroy(UIPanelKey key)
        {
            if (!_panelDic.ContainsKey(key))
                return;

            var ui = _panelDic[key];
            ui.SetActive(false);
            PrefabModule.Destroy(ui.gameObject);
        }

        public UINodeObject OpenNode(UINodeObject parent, NodeKey key, params object[] args)
        {
            var parentTrans = parent != null ? parent.transform : null;
            var go = PrefabModule.LoadSync($"UI/{key}.prefab", parentTrans);
            go.transform.SetAsLastSibling();
            var nodeObj = go.GetComponent<UINodeObject>();
            nodeObj.Open(args);
            return nodeObj;
        }

        public void CloseNode(UINodeObject node)
        {
            if (node.Key.GetType() != typeof(UINodeKey))
            {
                FrameDebugger.LogError($"key:{node.Key}; 这不是 UINodeKey ！！！！！");
                return;
            }

            node.SetActive(false);
        }

        public void RemoveNode(UINodeObject node)
        {
            node.SetActive(false);
            PrefabModule.Destroy(node.gameObject);
        }

        private int GetCurrentLayerSort(UIPanelLayer layer)
        {
            if (!_layerSortIndexDic.ContainsKey(layer))
            {
                _layerSortIndexDic.Add(layer, (int) (layer) * 10000);
            }

            return ++_layerSortIndexDic[layer];
        }
    }
}
