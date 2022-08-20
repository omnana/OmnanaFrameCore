using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NodeTree
{
    public interface INode
    {
        void Init();
        void Enable();
        void Open(params object[] contexts);
        void Update();
        void LateUpdate();
        void Disable();
        void Destroy();
    }
}
