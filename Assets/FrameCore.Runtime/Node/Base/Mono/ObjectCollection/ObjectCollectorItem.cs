using UnityEngine;

namespace FrameCore.Runtime
{
    [System.Serializable]
    public class ObjectCollectorItem
    {
        [SerializeField]
        public string name;

        [SerializeField]
        public GameObject component;
    }
}
