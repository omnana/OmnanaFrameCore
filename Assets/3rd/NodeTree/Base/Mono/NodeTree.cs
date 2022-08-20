using System.Collections.Generic;
using UnityEngine;

namespace NodeTree
{
    public class NodeTree : MonoBehaviour
    {
        private NodeBase _root;
        private Stack<NodeBase> _stack;
        
        void Start()
        {
            _root = new NodeBase();
        }

        private void Update()
        {
            _stack.Clear();
            _stack.Push(_root);
            while (_stack.Count > 0)
            {
                var node = _stack.Pop();
                node.OnUpdate();
                for (int i = 0, cnt = node.Childes.Count; i < cnt; i++)
                {
                    _stack.Push(node.Childes[i]);
                }
            }
        }

        private void LateUpdate()
        {
            _stack.Clear();
            _stack.Push(_root);
            while (_stack.Count > 0)
            {
                var node = _stack.Pop();
                node.OnLateUpdate();
                for (int i = 0, cnt = node.Childes.Count; i < cnt; i++)
                {
                    _stack.Push(node.Childes[i]);
                }
            }
        }
    }
}
