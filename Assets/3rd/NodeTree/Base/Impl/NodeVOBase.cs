using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NodeTree
{
    public class NodeVOBase
    {
        public virtual void OnNodeGetter(){}
        public virtual void OnInit(){}
        public virtual void OnDestroy(){}
        
        protected virtual void CreateNode(NodeKey nodeKey, params object[] contexts){}
        
        protected virtual void OpenNode(NodeKey nodeKey, params object[] contexts){}
        
        protected virtual void Remove(NodeVOBase nodeVoBase){}
    }
}