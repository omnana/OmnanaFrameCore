using System;
using System.Collections.Generic;
using FrameCore.Runtime;
using UnityEditor;
using UnityEngine;


namespace FrameCore.Editor
{
    public class CodeGenEditor
    {
        private readonly Dictionary<Type, Type> _typeDic = new Dictionary<Type, Type>
        {
            {typeof(MapNodeObject), typeof(NodeObjectElementDrawer)},
            {typeof(UINodeObject), typeof(NodeObjectElementDrawer)},
            {typeof(EntityObject), typeof(EntityObjectElementDrawer)},
        };

        private readonly GameObject _go;
        private readonly string _name;
        private readonly string _product;
        private readonly Type _objectType;
        private readonly IElementDrawer _elementDrawer;

        public CodeGenEditor(GameObject go, Type objectType)
        {
            _go = go;
            _objectType = objectType;
            _product = GameConfigHelper.GetProduct();
            _name = CodeWriteHelpUtil.GetPrefabOriginalName(go);
            if (_typeDic.ContainsKey(objectType))
            {
                _elementDrawer = (IElementDrawer) Activator.CreateInstance(_typeDic[objectType]);
            }
            else
            {
                Debug.LogError($"objectType : {objectType} 没有配置对应IElementDrawer");
            }
        }

        /// <summary>
        /// 生成器面板
        /// </summary>
        public void DrawCode()
        {
            GUILayout.BeginVertical("HelpBox");
            _elementDrawer.Draw(_go, _product, _name);
            GUILayout.Space(5);
            GUIStyle style = new GUIStyle("Button")
            {
                alignment = TextAnchor.MiddleCenter,
                fontStyle = FontStyle.BoldAndItalic,
                fontSize = 15,
            };
            GUI.color = Color.green;
            if (GUILayout.Button("Generate", style, GUILayout.Height(25)))
            {
                GenerateUtility.Generate(_go, _objectType);
                AssetDatabase.Refresh();
            }

            GUI.color = Color.white;
            GUILayout.EndVertical();
        }
    }
}