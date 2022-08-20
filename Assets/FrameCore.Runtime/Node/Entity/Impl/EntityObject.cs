using UnityEngine;

namespace FrameCore.Runtime
{
    public class EntityObject : MonoBehaviour, IGoGetter
    {
        [SerializeField] private ObjectCollector m_objectCollector = new ObjectCollector();

        public T GetGo<T>(string key) where T : Object => m_objectCollector[key] as T;
    }
}