using UnityEngine;

namespace FrameCore.Runtime
{
    public class DebugController : MonoBehaviour
    {
        private GameObject _consoleGo;
        private float _lastPreTimestamp;
        
        public static void New()
        {
            var go = new GameObject("DebugController");
            go.AddComponent<DebugController>();
        }
        
        private void Awake()
        {
           var prefab = IdealResource.Load<GameObject>("Prefab\\IngameDebugConsole.prefab");
           _consoleGo = Instantiate(prefab, transform, false);
           _consoleGo.SetActive(true);
           DontDestroyOnLoad(gameObject);
           transform.localScale = Vector3.zero;
        }

        private void Update()
        {
            if (Input.GetKey(KeyCode.LeftAlt) && Input.GetMouseButtonDown(0))
            {
                if (transform.localScale == Vector3.zero)
                {
                    transform.localScale = Vector3.one;
                }
                else
                {
                    transform.localScale = Vector3.zero;
                }
            }
        }
    }
}
