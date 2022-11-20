using FrameCore.Utility;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

// 目前只提供同步
public static class IdealResource
{
    public static Object Load(string assetPath)
    {
        var resPath = FileUtility.GetFilePathWithoutExtension(assetPath);
        var result = Resources.Load(resPath);
        if (result != null)
            return result;

#if UNITY_EDITOR
        assetPath = $"Assets/PreResources/{assetPath.Replace("\\", "/")}";
        return AssetDatabase.LoadAssetAtPath<Object>(assetPath);
#else
            return null;
#endif
    }

    public static T Load<T>(string assetPath) where T : Object
    {
        var resPath = FileUtility.GetFilePathWithoutExtension(assetPath);
        var result = Resources.Load<T>(resPath);
        if (result != null)
            return result;

#if UNITY_EDITOR
        assetPath = $"Assets/PreResources/{assetPath.Replace("\\", "/")}";
        return AssetDatabase.LoadAssetAtPath<T>(assetPath);
#else
            return null;
#endif
    }

    public static void UnLoad(Object target)
    {
        Resources.UnloadAsset(target);
    }
}
