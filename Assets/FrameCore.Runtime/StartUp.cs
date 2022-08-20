using UnityEngine;

namespace FrameCore.Runtime
{
    internal static class StartUp
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Do()
        {
            if (!IsSceneValid())
                return;
            
            AppManager.Inst.Start();
        }

        private static bool IsSceneValid()
        {
            return UnityEngine.SceneManagement.SceneManager.GetActiveScene().name.Equals("StartUp");
        }
    }
}
