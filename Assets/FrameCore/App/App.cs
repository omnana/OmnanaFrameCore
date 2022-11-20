using System;
using System.Reflection;
using FrameCore.Util;
using UnityEngine;

namespace FrameCore.Runtime
{
    public class App : MonoBehaviour
    {
        private Action<float> _updateAction;
        private Action<float> _lateUpdateAction;
        private Action<float> _fixedUpdateAction;
        private Action _destroyAction;

        #region 生命周期

        private void Awake()
        {
            DontDestroyOnLoad(this);
            ExtCore.Init(this);
        }

        private void Update()
        {
            _updateAction?.Invoke(Time.deltaTime);
            ExtCore.Update();
        }

        private void LateUpdate()
        {
            _lateUpdateAction?.Invoke(Time.deltaTime);
            ExtCore.LateUpdate();
        }

        private void FixedUpdate()
        {
            _fixedUpdateAction?.Invoke(Time.fixedDeltaTime);
            ExtCore.FixedUpdate();
        }

        private void OnDestroy()
        {
            _destroyAction?.Invoke();
        }

        #endregion

        public void Load()
        {
            IocContainer.Resolve<IAssetModule>().Load();
            var dllName = GameConfigHelper.GetProduct();
#if UNITY_EDITOR
            var bytes = System.IO.File.ReadAllBytes(
                DirectoryUtil.GetProjectDirectory($"/Library/ScriptAssemblies/{dllName}.dll"));
#else
            var asset = IocContainer.Resolve<IAssetModule>().LoadSync<TextAsset>($"dll/{dllName}.dll.bytes");
            if (asset == null)
                return;

            var bytes = asset.bytes;
#endif
            Assembly assembly = Assembly.Load(bytes);
            ExecuteAssembly(dllName, assembly);
        }

        private void ExecuteAssembly(string dllName, Assembly assembly)
        {
            var entranceType = assembly.GetType($"{dllName}.GameEntry");
            if (entranceType == null)
            {
                FrameDebugger.LogError($"请在{dllName}程序集内创建 GameEntry 入口类！！");
                return;
            }

            var method = entranceType.GetMethod("GetInitDelegate");
            if (method == null)
            {
                FrameDebugger.LogError($"请在{dllName}.GameLogic程序集内，创建静态函数 Init，以及委托 GetInitDelegate");
                return;
            }

            var init = (Action) method.Invoke(null, null);
            init?.Invoke();

            method = entranceType.GetMethod("GetUpdateDelegate");
            if (method == null)
            {
                FrameDebugger.LogError($"请在{dllName}.GameLogic程序集内，创建静态函数 Update，以及委托 GetUpdateDelegate");
                return;
            }

            _updateAction = (Action<float>) method.Invoke(null, null);

            method = entranceType.GetMethod("GetLateUpdateDelegate");
            if (method == null)
            {
                FrameDebugger.LogError($"请在{dllName}.GameLogic程序集内，创建静态函数 LateUpdate，以及委托 GetLateUpdateDelegate");
                return;
            }

            _lateUpdateAction = (Action<float>) method.Invoke(null, null);

            method = entranceType.GetMethod("GetFixedUpdateDelegate");
            if (method == null)
            {
                FrameDebugger.LogError($"请在{dllName}.GameLogic程序集内，创建静态函数 FixedUpdate，以及委托 GetFixedUpdateDelegate");
                return;
            }

            _fixedUpdateAction = (Action<float>) method.Invoke(null, null);

            method = entranceType.GetMethod("GetDestroyDelegate");
            if (method == null)
            {
                FrameDebugger.LogError($"请在{dllName}.GameLogic程序集内，创建静态函数 Destroy，以及委托 GetDestroyDelegate");
                return;
            }

            _destroyAction = (Action) method.Invoke(null, null);
        }
    }
}
