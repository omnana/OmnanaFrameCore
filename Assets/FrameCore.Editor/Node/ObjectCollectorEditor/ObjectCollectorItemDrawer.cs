using FrameCore.Runtime;
using UnityEditor;
using UnityEngine;

namespace FrameCore.Editor
{
    [CustomPropertyDrawer(typeof(ObjectCollectorItem))]
    public class ObjectCollectorItemDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            using (new EditorGUI.PropertyScope(position, label, property))
            {
                var viewRect = position;
                Rect nameRect = viewRect;
                nameRect.width = viewRect.width * 0.4f;
                Rect componentRect = new Rect(position)
                {
                    x = viewRect.width * 0.5f + 20,
                    width = viewRect.width * 0.5f
                };

                SerializedProperty nameProperty = property.FindPropertyRelative("name");
                SerializedProperty componentProperty = property.FindPropertyRelative("component");
                nameProperty.stringValue = EditorGUI.TextField(nameRect, "", nameProperty.stringValue);
#pragma warning disable 0618
                componentProperty.objectReferenceValue = EditorGUI.ObjectField(componentRect,
                    componentProperty.objectReferenceValue, typeof(GameObject));
#pragma warning restore 0618
            }
        }
    }
}