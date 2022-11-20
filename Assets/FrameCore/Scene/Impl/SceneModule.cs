using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FrameCore.Runtime
{
    public class SceneModule : ISceneModule, IMonoGetter
    {
        private MonoBehaviour _app;

        public void GetMono(MonoBehaviour mono)
        {
            _app = mono;
        }

        public void LoadSceneSync(string sceneName)
        {
#if UNITY_EDITOR && !AB_DEBUG
#else
            var abName = IocContainer.Resolve<IAssetModule>().GetAssetAbName($"Scenes/{sceneName}.unity");
            IocContainer.Resolve<IAssetBundleModule>().LoadSync(abName);
#endif
            SceneManager.LoadScene(sceneName);
        }

        public void LoadSceneAsync(string sceneName, Action<float> onProgress, Action onComplete)
        {
#if UNITY_EDITOR && !AB_DEBUG
#else
            var abName = IocContainer.Resolve<IAssetModule>().GetAssetAbName($"Scenes/{sceneName}.unity");
            IocContainer.Resolve<IAssetBundleModule>().LoadSync(abName);
#endif
            _app.StartCoroutine(DoLoadSceneAsync(sceneName, onProgress, onComplete));
        }

        public void UnLoad(string sceneName, Action<float> onProgress, Action onComplete)
        {
#if UNITY_EDITOR && !AB_DEBUG
#else
            var abName = IocContainer.Resolve<IAssetModule>().GetAssetAbName($"Scenes/{sceneName}.unity");
            IocContainer.Resolve<IAssetBundleModule>().LoadSync(abName);
#endif
            _app.StartCoroutine(DoUnLoadSceneAsync(sceneName, onProgress, onComplete));
        }

        private IEnumerator DoLoadSceneAsync(string sceneName, Action<float> onProgress, Action onComplete)
        {
            var req = SceneManager.LoadSceneAsync(sceneName);
            onProgress?.Invoke(req.progress);
            while (!req.isDone) yield return null;
            onComplete?.Invoke();
        }

        private IEnumerator DoUnLoadSceneAsync(string sceneName, Action<float> onProgress, Action onComplete)
        {
            var req = SceneManager.UnloadSceneAsync(sceneName);
            onProgress?.Invoke(req.progress);
            while (!req.isDone) yield return null;
            onComplete?.Invoke();
        }
    }
}
