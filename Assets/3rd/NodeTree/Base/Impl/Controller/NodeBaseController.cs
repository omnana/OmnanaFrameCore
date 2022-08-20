using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NodeTree
{
    public abstract class NodeBaseController
    {
        public virtual void OnInit()
        {

        }

        public virtual void OnEnable()
        {

        }

        public virtual void OnOpen(params object[] contexts)
        {

        }

        public virtual void OnUpdate()
        {

        }

        public virtual void OnLateUpdate()
        {

        }

        public virtual void OnDisable()
        {

        }

        public virtual void OnDestroy()
        {

        }
    }
}