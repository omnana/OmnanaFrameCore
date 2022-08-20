using System.Reflection;
using FrameCore.Runtime;
using UnityEditor;
using UnityEngine;

namespace FrameCore.Editor
{
    [CustomEditor(typeof(StaticNodeCollector))]
    public class StaticNodeCollectorInspector : UnityEditor.Editor
    {
        private ObjectCollector _objectCollector;
        private BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
        
        private void OnEnable()
        {
            var collector = target as StaticNodeCollector;
            var nodeObject = collector.GetComponent<NodeObject>();
            var value = nodeObject.GetType().BaseType.GetField("m_objectCollector", flags);
            _objectCollector = value.GetValue(nodeObject) as ObjectCollector;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (!Application.isPlaying)
            {
                var collector = target as StaticNodeCollector;
                collector.NodeObjects.Clear();
                foreach (var obj in _objectCollector.values)
                {
                    var objType = obj.GetType();
                    if (objType == typeof(MapNodeObject))
                    {
                        collector.NodeObjects.Add((MapNodeObject)obj);
                    }
                    else if (objType == typeof(UINodeObject))
                    {
                        collector.NodeObjects.Add((UINodeObject)obj);
                    }
                }
            
                serializedObject.Update();
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}
