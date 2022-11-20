using System;

namespace FrameCore.Runtime
{
    public interface ISceneModule
    {
        void LoadSceneSync(string sceneName);
        void LoadSceneAsync(string sceneName, Action<float> onProgress = null, Action onComplete = null);
        void UnLoad(string sceneName, Action<float> onProgress = null, Action onComplete = null);
    }
}
