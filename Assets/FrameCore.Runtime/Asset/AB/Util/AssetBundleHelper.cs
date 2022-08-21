using FrameCore.Util;
using UnityEngine;

namespace FrameCore.Runtime
{
    public static class AssetBundleHelper
    {
        public const int Max_Loading_Count = 10;
        public const string StreamAsset = "StreamingAssets";
        public const string MainResFolderName = "RawResources";

        /// <summary>
        /// ab总manifest名称
        /// </summary>
        public static string GetManifestPath()
        {
           return $"{GetStreamAssetPath()}/StreamingAssets";
        }

        public static string GetAbPath(string abName)
        {
            return $"{GetStreamAssetPath()}/{abName.ToLower()}";
        }

        // 编辑器打包资源输出文件夹
        public static string GetOutputFolderEditor()
        {
            return $"{DirectoryUtil.GetProjectDirectory(StreamAsset)}";
        }
        
        // 获取资源文件的统一前缀
        public static string GetAbFolderPrefix()
        {
            return $"Assets/{MainResFolderName}/";
        }

        // 打包的资源路径
        public static string GetDestFolder()
        {
            return $"{Application.dataPath}/RawResources";
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
