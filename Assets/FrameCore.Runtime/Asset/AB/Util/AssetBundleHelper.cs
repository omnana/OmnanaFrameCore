using FrameCore.Util;
using UnityEngine;

namespace FrameCore.Runtime
{
    public static class AssetBundleHelper
    {
        public const int Max_Loading_Count = 10;
        public const string StreamAsset = "StreamingAssets";

        /// <summary>
        /// ab总manifest名称
        /// </summary>
        public static string GetManifestPath()
        {
           return $"{GetStreamAssetPath()}/RawResources/RawResources";
        }

        /// <summary>
        /// 包前缀
        /// </summary>
        /// <param name="folder"></param>
        /// <returns></returns>
        public static string GetRawResourcesPrefix(string folder)
        {
            return $"{folder}/RawResources";
        }

        public static string GetAbPath(string abName)
        {
            return $"{GetStreamAssetPath()}/RawResources/{abName.ToLower()}";
        }

        // 打包路径
        public static string GetBuildFolder(string folder)
        {
            return $"{Application.dataPath}/{folder}/RawResources";
        }

        private static string GetStreamAssetPath()
        {
            switch (Application.platform)
            {
                case RuntimePlatform.WindowsEditor:
                    return DirectoryUtil.GetProjectDirectory(StreamAsset);
                case RuntimePlatform.WindowsPlayer:
                    return Application.streamingAssetsPath;
                case RuntimePlatform.Android:
                case RuntimePlatform.IPhonePlayer:
                    return Application.persistentDataPath;
                default:
                    return Application.streamingAssetsPath;
            }
        }

        private static string GetRunTimePlatformName()
        {
            switch (Application.platform)
            {
                case RuntimePlatform.Android:
                    return "Android";
                case RuntimePlatform.IPhonePlayer:
                    return "Ios";
                case RuntimePlatform.WindowsPlayer:
                case RuntimePlatform.WindowsEditor:
                    return "Win";
            }

            return string.Empty;
        }
    }
}
