using System;
using System.Linq;
using System.Reflection;
using FrameCore.Runtime;
using UnityEditor;

namespace FrameCore.Editor
{
    [CustomPropertyDrawer(typeof(ObjectCollector))]
    public partial class ObjectCollectorEditor
    {
        private static BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

        public static void ObjectCollectorDrawer(SerializedProperty property, Type objectType, Type collectorType,
            bool isGenerate, Type[] blackTypes = null)
        {
            var ob = property.serializedObject.targetObject;
            var selfFields = ob.GetType().GetFields(flags);
            var baseFields = ob.GetType().BaseType.GetFields(flags);
            var fields = selfFields.Concat(baseFields).ToArray();
            foreach (var field in fields)
            {
                if (field.FieldType == typeof(ObjectCollector))
                {
                    var objectCollector = field.GetValue(ob) as ObjectCollector;
                    objectCollector.objectType = objectType;
                    objectCollector.collectorType = collectorType;
                    objectCollector.isGenerate = isGenerate;
                    objectCollector.blackTypes = blackTypes;
                    break;
                }
            }

            EditorGUILayout.PropertyField(property);
        }
    }
}