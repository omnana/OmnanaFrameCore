using System.Collections.Generic;
using UnityEngine;

namespace FrameCore.Runtime
{
    public class GoPool
    {
        private readonly Stack<GameObject> _stack = new Stack<GameObject>();
        
        public GameObject New(GameObject prefab, Transform parent)
        {
            var item = Object.Instantiate(prefab, parent, false);
            return item;
        }
    
        public GameObject Get(GameObject prefab, Transform parent)
        {
            if (_stack.Count > 0)
                return _stack.Pop();

            var item = Object.Instantiate(prefab, parent, false);
            return item;
        }

        public GameObject Get()
        {
            return _stack.Pop();
        }

        public void Recycle(GameObject item)
        {
            _stack.Push(item);
        }

        public void Clear()
        {
            while (_stack.Count > 0)
            {
                var item = _stack.Pop();
                Object.Destroy(item);
            }
        }

        public int Count => _stack.Count;
    }
}
