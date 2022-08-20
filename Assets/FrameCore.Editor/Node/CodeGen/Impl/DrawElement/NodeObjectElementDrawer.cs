using UnityEditor;
using UnityEngine;
using FrameCore.Runtime;

namespace FrameCore.Editor
{
    public class NodeObjectElementDrawer : IElementDrawer
    {
        public void Draw(GameObject go, string product, string name)
        {
            var nodeObject = go.GetComponent<NodeObject>();
            Object controllerObj = null;
            Object voObj = null;
            Object voGenObj = null;
            Object builderObj = null;
            var mainFolder = nodeObject.NodeType switch
            {
                NodeType.MapStage => CodeWriteHelpUtil.GetPrefabMapFolder(product, "Map/", go, true),
                NodeType.MapNode => CodeWriteHelpUtil.GetPrefabMapFolder(product, "Map/", go),
                NodeType.UIPanel => CodeWriteHelpUtil.GetPrefabUIFolder(product, go, true),
                NodeType.UINode => CodeWriteHelpUtil.GetPrefabUIFolder(product, go),
                _ => string.Empty
            };

            if (!string.IsNullOrEmpty(mainFolder))
            {
                var ctrlCodePath = $"{mainFolder}/Controllers/{name}Controller.cs";
                var voCodePath = $"{mainFolder}/VO/{name}VO.cs";
                var voCodeGenPath = $"{mainFolder}/VO/Gen/{name}VO.Gen.cs";
                var builderPath = $"{mainFolder}/{name}Builder.cs";
                controllerObj = AssetDatabase.LoadAssetAtPath<Object>(ctrlCodePath);
                voObj = AssetDatabase.LoadAssetAtPath<Object>(voCodePath);
                voGenObj = AssetDatabase.LoadAssetAtPath<Object>(voCodeGenPath);
                builderObj = AssetDatabase.LoadAssetAtPath<Object>(builderPath);
            }
        
            GUILayout.Space(5);
        
            ///////////// Controller.cs
            EditorGUILayout.BeginHorizontal();
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.LabelField("Controller", GUILayout.Width(60));
            EditorGUILayout.ObjectField(controllerObj, typeof(UnityEngine.Object), true);
            EditorGUI.EndDisabledGroup();
        
            GUILayout.Space(10);
        
            EditorGUILayout.Separator();
            if (!controllerObj) EditorGUI.BeginDisabledGroup(true);
            if (GUILayout.Button("Open", GUILayout.Width(50)))
            {
                AssetDatabase.OpenAsset(controllerObj);
            }

            if (!controllerObj) EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndHorizontal();
            /////////////////////////////////

            ///////////// vo.cs
            EditorGUILayout.BeginHorizontal();
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.LabelField("VO", GUILayout.Width(60));
            EditorGUILayout.ObjectField(voObj, typeof(Object), true);
            EditorGUI.EndDisabledGroup();
            GUILayout.Space(10);
            EditorGUILayout.Separator();
            if (!voObj) EditorGUI.BeginDisabledGroup(true);

            if (GUILayout.Button("Open", GUILayout.Width(50)))
            {
                AssetDatabase.OpenAsset(voObj);
            }

            if (!voObj) EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndHorizontal();
            /////////////////////////////////

            ///////////// vo.Gen.cs
            EditorGUILayout.BeginHorizontal();
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.LabelField("VO.Gen", GUILayout.Width(60));
            EditorGUILayout.ObjectField(voGenObj, typeof(Object), true);
            EditorGUI.EndDisabledGroup();
            GUILayout.Space(10);
            EditorGUILayout.Separator();
            if (!voGenObj) EditorGUI.BeginDisabledGroup(true);

            if (GUILayout.Button("Open", GUILayout.Width(50)))
            {
                AssetDatabase.OpenAsset(voGenObj);
            }

            if (!voGenObj) EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndHorizontal();
            /////////////////////////////////

            ///////////// builder.cs
            EditorGUILayout.BeginHorizontal();
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.LabelField("builder", GUILayout.Width(60));
            EditorGUILayout.ObjectField(builderObj, typeof(Object), true);
            EditorGUI.EndDisabledGroup();
            GUILayout.Space(10);
            EditorGUILayout.Separator();
            if (!builderObj) EditorGUI.BeginDisabledGroup(true);

            if (GUILayout.Button("Open", GUILayout.Width(50)))
            {
                AssetDatabase.OpenAsset(builderObj);
            }

            if (!builderObj) EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndHorizontal();
            /////////////////////////////////
        
            var tip = "代码自动生成，controller，vo第一次自动生成，vo.Gen, builder每次自动生成！";
            GUILayout.Space(5);
            EditorGUILayout.HelpBox(tip, MessageType.Info);
        }
    }
}
