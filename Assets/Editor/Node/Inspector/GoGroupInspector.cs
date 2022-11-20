using FrameCore.Runtime;
using UnityEditor;
using UnityEngine;

namespace FrameCore.Editor
{
    [CustomEditor(typeof(GoGroup))]
    public class GoGroupInspector : UnityEditor.Editor
    {
        private void OnEnable()
        {
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var group = target as GoGroup;
            group.Group.Clear();
            foreach (var child in group.transform)
            {
                group.Group.Add(((Transform)child).gameObject);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
