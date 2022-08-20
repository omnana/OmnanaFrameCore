using System.Collections.Generic;
using UnityEngine;

namespace FrameCore.Runtime
{
    public interface IGoGroup
    {
        List<GameObject> Group { get; }
        GameObject GetGo(string goName);
    }

    public class GoGroup : MonoBehaviour, IGoGroup
    {
        [SerializeField] private List<GameObject> _group = new List<GameObject>();
        
        public List<GameObject> Group => _group;

        private readonly Dictionary<string, GameObject> _kvMap = new Dictionary<string, GameObject>();

        public GameObject this[string key] => _kvMap[key];

        private void Cache()
        {
            if (_kvMap.Count != _group.Count)
            {
                foreach (var go in _group)
                {
                    if (!_kvMap.ContainsKey(go.name))
                    {
                        _kvMap.Add(go.name, go);
                    }
                }
            }
        }

        public GameObject GetGo(string goName)
        {
            Cache();

            return !_kvMap.ContainsKey(goName) ? null : _kvMap[goName];
        }
    }
}