using System.Collections.Generic;
using UnityEngine;

namespace FrameCore.Runtime
{
    public class UIModule : IUIModule
    {
        private Dictionary<UIPanelKey, UINodeObject> _panelDic;
        private Dictionary<UIPanelLayer, int> _layerSortIndexDic;

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

                var go = IocContainer.Resolve<IPrefabModule>().LoadSync($"UI/{key}.prefab");
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
            IocContainer.Resolve<IPrefabModule>().Destroy(ui.gameObject);
        }

        public int OpenNode(UINodeObject parent, NodeKey key, params object[] args)
        {
            var parentTrans = parent != null ? parent.transform : null;
            var go = IocContainer.Resolve<IPrefabModule>().LoadSync($"Map/{key}.prefab", parentTrans);
            if (go == null)
                return 0;
            
            go.transform.SetAsLastSibling();
            var nodeObj = go.GetComponent<UINodeObject>();
            parent.AddChild(nodeObj);
            nodeObj.Open(args);
            return nodeObj.Id;
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
            if (node.Parent != null)
            {
                node.Parent.RemoveChild(node);
            }

            node.SetActive(false);
            IocContainer.Resolve<IPrefabModule>().Destroy(node.gameObject);
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
