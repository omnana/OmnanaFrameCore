using FrameCore.Runtime;
using UnityEditor;
using UnityEngine;
using UnityToolbarExtender;

namespace FrameCore.Editor
{
    [InitializeOnLoad]
    public class AssetEditor
    {
        static readonly string[,] RuntimeModeContent =
            {{"EditorAsset", "当前资源模式：EditorAsset\n可进行调式\n运行时不可切换"}, {"AssetBundle", "当前资源模式：AssetBundle\n不可进行调试\n运行时不可切换"}};

        static AssetEditor()
        {
            ToolbarExtender.LeftToolbarGUI.Add(OnUpdate);
        }

        static void OnUpdate()
        {
            var btnStyle = new GUIStyle("Command")
            {
                fixedWidth = 88,
                fontSize = 13,
                alignment = TextAnchor.MiddleCenter,
                imagePosition = ImagePosition.ImageAbove,
                fontStyle = FontStyle.Bold
            };
            var runMode = (int) AssetHelper.AssetModel;
            EditorGUI.BeginDisabledGroup(Application.isPlaying);
            if (GUILayout.Button(new GUIContent(RuntimeModeContent[runMode, 0], RuntimeModeContent[runMode, 1]), btnStyle))
            {
                Debug.Log($"切换资源模式成功，当前模式：{RuntimeModeContent[runMode = runMode == 0 ? 1 : 0, 0]}");
                AssetHelper.SetAssetModel((AssetModel) runMode);
            }

            EditorGUI.EndDisabledGroup();
        }
    }
}
