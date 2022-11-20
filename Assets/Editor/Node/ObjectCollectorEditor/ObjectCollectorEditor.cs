using System;
using System.Linq;
using FrameCore.Runtime;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace FrameCore.Editor
{
    [CustomPropertyDrawer(typeof(ObjectCollector))]
    public partial class ObjectCollectorEditor : PropertyDrawer
    {
        private ReorderableList collectList;
        private SerializedProperty keys;
        private SerializedProperty values;

        private ObjectCollector objectCollector;
        private CodeGenEditor codeGenEditor;

        private void SetData(SerializedProperty property)
        {
            var ob = property.serializedObject.targetObject;
            var selfFields = ob.GetType().GetFields(flags);
            var baseFields = ob.GetType().BaseType.GetFields(flags);
            var fields = selfFields.Concat(baseFields).ToArray();

            foreach (var field in fields)
            {
                if (field.FieldType == typeof(ObjectCollector))
                {
                    objectCollector = field.GetValue(ob) as ObjectCollector;
                    break;
                }
            }
        }

        private CodeGenEditor GetCodeGenEditor(SerializedProperty property)
        {
            if (codeGenEditor == null)
            {
                var ob = property.serializedObject.targetObject;
                var go =
                    ob.GetType().GetProperty("gameObject", flags)
                        .GetValue(ob) as GameObject;
                codeGenEditor = new CodeGenEditor(go, objectCollector.objectType);
            }

            return codeGenEditor;
        }

        private void OnInit(SerializedProperty property)
        {
            SetData(property);
            collectList.headerHeight = 0;
            // 绘制
            collectList.drawElementCallback = (Rect rect, int index, bool selected, bool focused) =>
            {
                var key = keys.GetArrayElementAtIndex(index);
                var value = values.GetArrayElementAtIndex(index);

                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(10);
                var copyRect = new Rect(rect.x, rect.y + 2, rect.width / 3, EditorGUIUtility.singleLineHeight);
                if (GUI.Button(copyRect, "", "HelpBox"))
                {
                    TextEditor textEditor = new TextEditor();
                    textEditor.text = key.stringValue;
                    textEditor.OnFocus();
                    textEditor.Copy();
                    Debug.Log($"序列化器Key复制成功！Key：{key.stringValue}");
                }

                EditorGUI.BeginDisabledGroup(true);
                var keyRect = new Rect(rect.x, rect.y + 2, rect.width / 3, EditorGUIUtility.singleLineHeight);
                EditorGUI.TextField(keyRect, key.stringValue);
                EditorGUI.EndDisabledGroup();
                var valueRect = new Rect(rect.x + rect.width / 3 + 10, rect.y + 2, rect.width - (rect.width / 3 + 15),
                    EditorGUIUtility.singleLineHeight);
                Type type = objectCollector.collectorType;
                var tmp = value.objectReferenceValue;
                value.objectReferenceValue = EditorGUI.ObjectField(valueRect, value.objectReferenceValue, type, true);

                if (tmp != value.objectReferenceValue)
                {
                    if (value.objectReferenceValue != null)
                    {
                        if (CheckForSameValue(value.objectReferenceValue, index))
                        {
                            value.objectReferenceValue = null;
                            key.stringValue = "";
                        }
                        else
                        {
                            string name = value.objectReferenceValue.name.Replace(" ", "");
                            if (!CheckForReName(name, index, value.objectReferenceValue))
                                key.stringValue = name;
                            else
                            {
                                value.objectReferenceValue = null;
                                key.stringValue = "";
                            }
                        }
                    }
                    else
                    {
                        key.stringValue = string.Empty;
                    }
                }
                else
                {
                    if (value.objectReferenceValue != null)
                    {
                        string name = value.objectReferenceValue.name.Replace(" ", "");
                        key.stringValue = name;
                    }
                    else
                    {
                        key.stringValue = string.Empty;
                    }
                }

                EditorGUILayout.EndHorizontal();
            };

            collectList.onAddCallback = list =>
            {
                values.arraySize++;
                keys.arraySize = values.arraySize;
                values.GetArrayElementAtIndex(values.arraySize - 1).objectReferenceValue = null;
                keys.GetArrayElementAtIndex(values.arraySize - 1).stringValue = "";
            };

            collectList.onRemoveCallback = list =>
            {
                var index = list.index;
                if (list.index == -1)
                {
                    index = values.arraySize - 1;
                }

                values.GetArrayElementAtIndex(index).objectReferenceValue = null;
                keys.GetArrayElementAtIndex(index).stringValue = null;
                values.DeleteArrayElementAtIndex(index);
                keys.DeleteArrayElementAtIndex(index);
            };
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            keys = property.FindPropertyRelative("keys");
            values = property.FindPropertyRelative("values");
            GUILayout.BeginVertical("HelpBox");

            EditorGUILayout.Space();
            if (collectList == null)
            {
                collectList = new ReorderableList(property.serializedObject, values, true, false, true, true);
                OnInit(property);
            }

            if (Application.isPlaying) EditorGUI.BeginDisabledGroup(true);

            string typeName = objectCollector.collectorType.Name;

            GUI.contentColor = Color.white;
            var dragArea = GUILayoutUtility.GetRect(0f, 45f);
            GUI.color = Color.grey;
            UnityEngine.Object ob =
                DragAreaGUI.ObjectField(dragArea, objectCollector.collectorType, objectCollector.blackTypes);
            if (ob != null)
                AddObjectToList(ob);
            GUI.color = Color.white;
            GUILayout.Space(8);

            collectList.DoLayoutList();

            GUILayout.EndVertical();

            if (objectCollector.isGenerate)
            {
                GetCodeGenEditor(property).DrawCode();
            }

            if (Application.isPlaying) EditorGUI.EndDisabledGroup();
        }

        private void AddObjectToList(UnityEngine.Object ob)
        {
            if (CheckForSameValue(ob, -1)) return;
            string name = ob.name.Replace(" ", "");
            if (CheckForReName(name, -1, ob)) return;
            values.arraySize++;
            keys.arraySize = values.arraySize;
            values.GetArrayElementAtIndex(values.arraySize - 1).objectReferenceValue = ob;
            keys.GetArrayElementAtIndex(values.arraySize - 1).stringValue = ob.name;
        }

        private bool CheckForSameValue(UnityEngine.Object value, int index)
        {
            for (int i = 0; i < values.arraySize; i++)
            {
                if (index == i) continue;
                var compareValue = values.GetArrayElementAtIndex(i).objectReferenceValue;
                if (compareValue == null) continue;
                if (value.GetInstanceID() == compareValue.GetInstanceID())
                {
                    EditorUtility.DisplayDialog("Warning",
                        $"已经添加过该{objectCollector.collectorType.Name}啦！",
                        "确定");
                    return true;
                }
            }

            return false;
        }

        private bool CheckForReName(string name, int index, UnityEngine.Object ob)
        {
            for (int i = 0; i < values.arraySize; i++)
            {
                if (i == index) continue;
                var value = values.GetArrayElementAtIndex(i);
                if (value.objectReferenceValue == null) continue;
                string compareName = value.objectReferenceValue.name.Replace(" ", "");
                if (!string.IsNullOrEmpty(compareName) && name == compareName)
                {
                    EditorUtility.DisplayDialog("Warning",
                        $"该{objectCollector.collectorType.Name}名称{name}重名啦！请修改GameObject名称。", "确定");
                    EditorGUIUtility.PingObject(ob);
                    return true;
                }
            }

            return false;
        }
    }
}
