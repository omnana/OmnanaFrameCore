using UnityEngine;

namespace FrameCore.Runtime
{
    // 暂时用这个
    public static class NetHelper
    {
        /// <summary>
        /// 是否有网络
        /// </summary>
        /// <returns></returns>
        public static bool IsNetConnect()
        {
            switch (Application.internetReachability)
            {
                // 没有网络
                case NetworkReachability.NotReachable:
                    return false;
                // 234G网络
                case NetworkReachability.ReachableViaCarrierDataNetwork:
                // wifi网络
                case NetworkReachability.ReachableViaLocalAreaNetwork:
                    return true;
                default:
                    return false;
            }
        }
    }
}
