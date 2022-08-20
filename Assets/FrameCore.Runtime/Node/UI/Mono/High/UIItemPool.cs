using System.Collections.Generic;
using UnityEngine;

namespace FrameCore.Runtime
{
    public class UIItemPool
    {
        private readonly Stack<GameObject> _stack = new Stack<GameObject>();
        
        public GameObject New(GameObject prefab, Transform parent)
        {
            if (_stack.Count > 0)
                return _stack.Pop();

            var item = Object.Instantiate(prefab, parent, false);
            return item;
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
    }
}