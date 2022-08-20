using FrameCore.Runtime;
using UnityEditor;
using UnityEngine;

namespace FrameCore.Editor
{
    [CustomEditor(typeof(MapNodeObject))]
    public class MapNodeObjectInspector : UnityEditor.Editor
    {
        private SerializedProperty _objectCollector;
        private MapNodeObject _mapNodeObject;
        private NodeType _lastNodeType;

        private void OnEnable()
        {
            _objectCollector = serializedObject.FindProperty("m_objectCollector");
            _mapNodeObject = target as MapNodeObject;
        }

        public override void OnInspectorGUI()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("MapKey");
            if (!Application.isPlaying)
            {
                var prefabName = CodeWriteHelpUtil.GetPrefabOriginalName(_mapNodeObject.gameObject);
                if (!string.IsNullOrEmpty(prefabName) && prefabName != _mapNodeObject.NodeKey)
                {
                    _mapNodeObject.NodeKey = prefabName;
                    EditorUtility.SetDirty(_mapNodeObject);
                }
            }
            GUILayout.TextField(_mapNodeObject.NodeKey);
            GUILayout.EndHorizontal();
        
            GUILayout.BeginHorizontal();
            GUILayout.Label("NodeType");
            _mapNodeObject.NodeType = (NodeType)EditorGUILayout.EnumPopup(_mapNodeObject.NodeType, GUILayout.Width(100));
            if (_lastNodeType != _mapNodeObject.NodeType)
            {
                EditorUtility.SetDirty(_mapNodeObject);
            }
            _lastNodeType = _mapNodeObject.NodeType;
            GUILayout.EndHorizontal();

            ObjectCollectorEditor.ObjectCollectorDrawer(_objectCollector, typeof(MapNodeObject), typeof(MonoBehaviour),
                true);
        
            serializedObject.ApplyModifiedProperties();
        }
    }
}
