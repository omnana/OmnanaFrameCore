using System.Collections.Generic;

namespace NodeTree
{
    public class NodeBase
    {
        public NodeVOBase VO;
        
        private List<NodeBaseController> _controllers;
        
        public NodeBase Parent { get; set; }
        public List<NodeBase> Childes { get; set; }

        public NodeBase()
        {
            _controllers = new List<NodeBaseController>();
            Childes = new List<NodeBase>();
        }
        
        public void OnInit()
        {
            VO.OnInit();
            foreach (var ctx in _controllers)
            {
                ctx.OnInit();
            }
        }

        public void OnEnable()
        {
            foreach (var ctx in _controllers)
            {
                ctx.OnEnable();
            }
        }

        public void OnOpen(params object[] contexts)
        {
            foreach (var ctx in _controllers)
            {
                ctx.OnOpen(contexts);
            }
        }

        public void OnUpdate()
        {
            
        }

        public void OnLateUpdate()
        {
            
        }

        public void OnDisable()
        {
            foreach (var ctx in _controllers)
            {
                ctx.OnDisable();
            }
        }

        public void OnDestroy()
        {
            VO.OnDestroy();
            foreach (var ctx in _controllers)
            {
                ctx.OnDestroy();
            }
        }

        public void AddEmptyController<T>(NodeObject nodeObject) where T : NodeBaseController
        {
            
        }

        public void RemoveEmptyController(NodeBaseController controller)
        {
            
        }
        
        public void InitStaticNode(NodeKey nodeKey){}
    }
}
