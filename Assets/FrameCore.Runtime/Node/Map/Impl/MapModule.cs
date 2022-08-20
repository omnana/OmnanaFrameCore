using System.Collections.Generic;
using UnityEngine;

namespace FrameCore.Runtime
{
    public class MapModule : IMapModule
    {
        private readonly Dictionary<MapStageKey, MapNodeObject> _mapStageDic;

        // private MapStageKey _runStage;
        private MapTree _mapTree;

        public MapModule()
        {
            _mapStageDic = new Dictionary<MapStageKey, MapNodeObject>();
        }

        public Camera CurrentStageCam => _mapTree.CurrentCam;

        public void RunStage(MapStageKey key)
        {
            if (_mapTree == null)
            {
                var root = GameObject.Find("App/Map");
                _mapTree = new GameObject("MapTree").AddComponent<MapTree>();
                _mapTree.Init(root);
            }

            if (key.GetType() != typeof(MapStageKey))
            {
                FrameDebugger.LogError($"key:{key}; 这不是 MapStageKey ！！！！！");
                return;
            }

            // if (_runStage != null)
            // {
            //     var current = _mapStageDic[_runStage];
            //     current.VO.SetActive(false);
            //     IocContainer.Resolve<IPrefabModule>().Destroy(current.VO.NodeObject.gameObject);
            // }

            if (!_mapStageDic.ContainsKey(key))
            {
                var go = IocContainer.Resolve<IPrefabModule>().LoadSync($"Map/{key}.prefab", _mapTree.Stage);
                if (go != null)
                {
                    go.transform.SetAsLastSibling();
                    go.SetActive(true);
                    var nodeObject = go.GetComponent<MapNodeObject>();
                    _mapStageDic.Add(key, nodeObject);
                }
            }

            _mapTree.CurrentCam = _mapStageDic[key].GetComponent<MapStage>().Camera;
            _mapStageDic[key].Open();
            // _runStage = key;
        }

        public void CloseStage(MapStageKey key)
        {
            if (!_mapStageDic.ContainsKey(key))
            {
                FrameDebugger.LogError($"没有stage:{key}在跑！！");
                return;
            }

            var stage = _mapStageDic[key];
            stage.SetActive(false);
        }

        public void DestroyStage(MapStageKey key)
        {
            if (!_mapStageDic.ContainsKey(key))
            {
                FrameDebugger.LogError($"没有stage:{key}在跑！！");
                return;
            }

            var stage = _mapStageDic[key];
            stage.SetActive(false);
            IocContainer.Resolve<IPrefabModule>().Destroy(stage.gameObject);
            _mapStageDic.Remove(key);
        }

        public int OpenNode(MapNodeObject parent, NodeKey nodeKey, params object[] args)
        {
            var parentTrans = parent == null ? _mapTree.Stage : parent.transform;
            var go = IocContainer.Resolve<IPrefabModule>().LoadSync($"Map/{nodeKey}.prefab", parentTrans);
            if (go == null)
                return 0;

            go.transform.SetAsLastSibling();
            var nodeObj = go.GetComponent<MapNodeObject>();
            parent.AddChild(nodeObj);
            nodeObj.Open(args);
            return nodeObj.Id;
        }

        public void CloseNode(MapNodeObject node)
        {
            node.SetActive(false);
        }

        public void RemoveNode(MapNodeObject node)
        {
            node.gameObject.SetActive(false);
            if (node.Parent != null)
            {
                node.Parent.RemoveChild(node);
            }

            IocContainer.Resolve<IPrefabModule>().Destroy(node.gameObject);
        }
    }
}
