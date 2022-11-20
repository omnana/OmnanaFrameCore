using FrameCore.Runtime;
using UnityEditor;
using UnityEngine;

namespace FrameCore.Editor
{
    public class EntityObjectElementDrawer : IElementDrawer
    {
        public void Draw(GameObject go, string product, string name)
        {
            var entityObject = go.GetComponent<EntityObject>();
            Object entityObj = null;
            Object entityGenObj = null;
            var mainFolder = CodeWriteHelpUtil.GetPrefabMapFolder(product, "Entity/", go, true);
            if (!string.IsNullOrEmpty(mainFolder))
            {
                var entityCodePath = $"{mainFolder}/{name}.cs";
                var entityGenPath = $"{mainFolder}/Gen/{name}.Gen.cs";
                entityObj = AssetDatabase.LoadAssetAtPath<Object>(entityCodePath);
                entityGenObj = AssetDatabase.LoadAssetAtPath<Object>(entityGenPath);
            }

            GUILayout.Space(5);

            ///////////// Entity.cs
            EditorGUILayout.BeginHorizontal();
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.LabelField("Entity", GUILayout.Width(60));
            EditorGUILayout.ObjectField(entityObj, typeof(Object), true);
            EditorGUI.EndDisabledGroup();

            GUILayout.Space(10);

            EditorGUILayout.Separator();
            if (!entityObj) EditorGUI.BeginDisabledGroup(true);
            if (GUILayout.Button("Open", GUILayout.Width(50)))
            {
                AssetDatabase.OpenAsset(entityObj);
            }

            if (!entityObj) EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndHorizontal();
            /////////////////////////////////
        
        
            ///////////// Entity.Gen.cs
            EditorGUILayout.BeginHorizontal();
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.LabelField("Entity.Gen", GUILayout.Width(60));
            EditorGUILayout.ObjectField(entityGenObj, typeof(Object), true);
            EditorGUI.EndDisabledGroup();

            GUILayout.Space(10);

            EditorGUILayout.Separator();
            if (!entityGenObj) EditorGUI.BeginDisabledGroup(true);
            if (GUILayout.Button("Open", GUILayout.Width(50)))
            {
                AssetDatabase.OpenAsset(entityGenObj);
            }

            if (!entityGenObj) EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndHorizontal();
            /////////////////////////////////

            GUILayout.Space(5);
            EditorGUILayout.HelpBox("代码自动生成，Entity第一次自动生成，Entity.Gen每次自动生成！", MessageType.Info);
        }
    }
}
