using System;

namespace GameLogic
{
    /// 提交给FrameCore的Delegate
    public partial class Entrance
    {
        public static Action GetInitDelegate() => Init;
        public static Action<float> GetUpdateDelegate() => Update;
        public static Action<float> GetLateUpdateDelegate() => LateUpdate;
        public static Action<float> GetFixedUpdateDelegate() => FixedUpdate;
        public static Action GetDestroyDelegate() => Destroy;
    }
}
