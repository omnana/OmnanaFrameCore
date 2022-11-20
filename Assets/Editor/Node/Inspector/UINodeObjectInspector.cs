using FrameCore.Runtime;
using UnityEditor;
using UnityEngine;

namespace FrameCore.Editor
{
    [CustomEditor(typeof(UINodeObject), true)]
    public class UINodeObjectInspector : UnityEditor.Editor
    {
        private SerializedProperty _objectCollector;
        private UINodeObject _uiNodeObject;
        private NodeType _lastNodeType;
        private UIPanelLayer _lastLayer;

        private void OnEnable()
        {
            _objectCollector = serializedObject.FindProperty("m_objectCollector");
            _uiNodeObject = target as UINodeObject;
        }

        public override void OnInspectorGUI()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("UIKey");
            if (!Application.isPlaying)
            {
                var prefabName = CodeWriteHelpUtil.GetPrefabOriginalName(_uiNodeObject.gameObject);
                if (!string.IsNullOrEmpty(prefabName) && prefabName != _uiNodeObject.NodeKey)
                {
                    _uiNodeObject.NodeKey = prefabName;
                    EditorUtility.SetDirty(_uiNodeObject);
                }
            }
            GUILayout.TextField(_uiNodeObject.NodeKey);
            GUILayout.EndHorizontal();
        
            GUILayout.BeginHorizontal();
            GUILayout.Label("NodeType");
            _uiNodeObject.NodeType = (NodeType)EditorGUILayout.EnumPopup(_uiNodeObject.NodeType, GUILayout.Width(100));
            if (_lastNodeType != _uiNodeObject.NodeType)
            {
                EditorUtility.SetDirty(_uiNodeObject);
            }
            _lastNodeType = _uiNodeObject.NodeType;
            GUILayout.EndHorizontal();

            if (_uiNodeObject.NodeType == NodeType.UINode || _uiNodeObject.NodeType == NodeType.UIPanel)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("UILayer");
                _uiNodeObject.UILayer =
                    (UIPanelLayer) EditorGUILayout.EnumPopup(_uiNodeObject.UILayer, GUILayout.Width(100));
                if (_lastLayer != _uiNodeObject.UILayer)
                {
                    EditorUtility.SetDirty(_uiNodeObject);
                }
                _lastLayer = _uiNodeObject.UILayer;
                GUILayout.EndHorizontal();
            }
        
            ObjectCollectorEditor.ObjectCollectorDrawer(_objectCollector, typeof(UINodeObject), typeof(MonoBehaviour),
                true);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
