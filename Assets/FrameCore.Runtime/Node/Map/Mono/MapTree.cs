using UnityEngine;

namespace FrameCore.Runtime
{
    public class MapTree : MonoBehaviour
    {
        public Transform Stage;

        public Camera CurrentCam;
        
        public void Init(GameObject root)
        {
            if (root != null)
            {
                transform.SetParent(root.transform, false);
            }

            Stage = new GameObject("MapStages").transform;
            Stage.SetParent(transform, false);
        }
    }
}