using System;

namespace FrameCore.Runtime
{
    public class AppManager
    {
        public static AppManager Inst => _instance ??= new AppManager();
        private static AppManager _instance;
        private App _app;

        public void Start()
        {
            _app = Framework.Start();
            _app.Load();
        }

        public void Restart()
        {
            GC.Collect();
        }

        public void ShutDown()
        {
        }
    }
}
