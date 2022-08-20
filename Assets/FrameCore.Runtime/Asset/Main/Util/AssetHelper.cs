using UnityEditor;

namespace FrameCore.Runtime
{
    public enum AssetModel
    {
        Editor,
        AssetBundle,
    }

    public static class AssetHelper
    {
        public const string AssetBundleModel = "Is_AssetBundle";
        public const int UNLOAD_DELAY_TICK_BASE = 60;
        public const int LOADING_INTERVAL_MAX_COUNT = 15;

        public static bool IsAbMode => AssetModel == AssetModel.AssetBundle;

        #region 给编辑器用

        public static void SetAssetModel(AssetModel model)
        {
#if UNITY_EDITOR
            EditorPrefs.SetInt(AssetBundleModel, (int) model);
#endif
        }

        public static AssetModel AssetModel
        {
            get
            {
#if UNITY_EDITOR
                return (AssetModel) EditorPrefs.GetInt(AssetBundleModel, 0);
#else
                return AssetModel.AssetBundle;
#endif
            }
        }

        #endregion
    }
}