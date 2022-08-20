using FrameCore.Runtime;
using UnityEditor;
using UnityEngine;

namespace FrameCore.Editor
{
    [CustomEditor(typeof(EntityObject))]
    public class EntityObjectInspector : UnityEditor.Editor
    {
        private SerializedProperty _objectCollector;

        private EntityObject _entityObject;
    
        private void OnEnable()
        {
            _objectCollector = serializedObject.FindProperty("m_objectCollector");
            _entityObject = target as EntityObject;
        }
    
    
        public override void OnInspectorGUI()
        {
            // GUILayout.BeginHorizontal();
            // GUILayout.Label("EntityKey");
            // if (!Application.isPlaying)
            // {
            //     var prefabName = CodeWriteHelpUtil.GetPrefabOriginalName(_entityMono.gameObject);
            //     if (!string.IsNullOrEmpty(prefabName) && prefabName != _uiNodeObject.NodeKey)
            //     {
            //         _uiNodeObject.NodeKey = prefabName;
            //         EditorUtility.SetDirty(_entityMono);
            //     }
            // }
            // GUILayout.TextField(_entityMono.gameObject.name);
            // GUILayout.BeginHorizontal();
        
            ObjectCollectorEditor.ObjectCollectorDrawer(_objectCollector, typeof(EntityObject), typeof(MonoBehaviour),
                true);
    
            serializedObject.ApplyModifiedProperties();
        }
    }
}
