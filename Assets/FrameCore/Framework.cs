using UnityEngine;

namespace FrameCore.Runtime
{
    internal static class Framework
    {
        public static App Start()
        {
            Bind();
            return CreateApp();
        }

        private static void Bind()
        {
            IocContainer.Bind(typeof(IObserverModule)).To(typeof(ObserverModule));
            IocContainer.Bind(typeof(IDownloadModule)).To(typeof(DownloadModule));
            IocContainer.Bind(typeof(IAssetBundleModule)).To(typeof(AssetBundleModule));
            IocContainer.Bind(typeof(IAssetModule)).To(typeof(AssetModule));
            IocContainer.Bind(typeof(IResourceModule)).To(typeof(ResourceModule));
            IocContainer.Bind(typeof(IPrefabModule)).To(typeof(PrefabModule));
            IocContainer.Bind(typeof(ISceneModule)).To(typeof(SceneModule));
            IocContainer.Bind(typeof(IMapModule)).To(typeof(MapModule));
            IocContainer.Bind(typeof(IUIModule)).To(typeof(UIModule));

#if UNITY_EDITOR
            IocContainer.Bind(typeof(IEditorAssetModule)).To(typeof(EditorAssetModule));
#endif
        }

        private static App CreateApp()
        {
#if DEBUG_LOG
            DebugController.New(); // 打开方式：工具栏/Tools/宏设置/打印日志
            FrameDebugger.Log("进入Debug模式（左边Alt+鼠标左键，可以打开Debug面板）");
#endif
            var appPrefab = IdealResource.Load<GameObject>("App.prefab");
            var app = Object.Instantiate(appPrefab);
            app.name = "App";
            InitUIRoot(app);
            Object.DontDestroyOnLoad(app);
            return app.GetComponent<App>();
        }

        private static void InitUIRoot(GameObject app)
        {
            var uiRoot = app.transform.Find("UIRoot").GetComponent<UIRoot>();
            uiRoot.Init();
            uiRoot.CheckBarState();
        }
    }
}