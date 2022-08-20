using System.Collections.Generic;
using UnityEngine;

namespace FrameCore.Runtime
{
    public class StaticNodeCollector : MonoBehaviour
    {
        public List<NodeObject> NodeObjects => m_nodeObjects;
    
        [SerializeField] private List<NodeObject> m_nodeObjects = new List<NodeObject>();
    }
}
