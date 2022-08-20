// using UnityEditor;
// using UnityEngine;
//
// namespace DataTable.Editor
// {
//     [CustomEditor(typeof(DataTableConfigSo))]
//     public class DataTableSoInspector : UnityEditor.Editor
//     {
//         private SerializedProperty _destDirectorySp;
//         private SerializedProperty _csvFolderSp;
//
//         private void OnEnable()
//         {
//             _destDirectorySp = serializedObject.FindProperty("mScriptFolder");
//             _csvFolderSp = serializedObject.FindProperty("mCsvFolder");
//         }
//
//         // public override void OnInspectorGUI()
//         // {
//         //     var config = target as DataTableConfigSo;
//         //     if(config == null)
//         //         return;
//         //
//         //     // csv路径
//         //     EditorGUILayout.BeginHorizontal();
//         //     EditorGUILayout.LabelField("csv主文件夹", GUILayout.Width(100));
//         //     GUI.enabled = false;
//         //     {
//         //         _csvFolderSp.stringValue = EditorGUILayout.TextField(_csvFolderSp.stringValue);
//         //     }
//         //     GUI.enabled = true;
//         //     if (GUILayout.Button("选择"))
//         //     {
//         //         _csvFolderSp.stringValue = EditorUtility.OpenFolderPanel("select path", "", "");
//         //     }
//         //     EditorGUILayout.EndHorizontal();
//         //     
//         //     // 输出路径
//         //     EditorGUILayout.BeginHorizontal();
//         //     EditorGUILayout.LabelField("脚本主文件夹", GUILayout.Width(100));
//         //     GUI.enabled = false;
//         //     {
//         //         _destDirectorySp.stringValue = EditorGUILayout.TextField(_destDirectorySp.stringValue);
//         //     }
//         //     GUI.enabled = true;
//         //     if (GUILayout.Button("选择"))
//         //     {
//         //         _destDirectorySp.stringValue = EditorUtility.OpenFolderPanel("select path", "", "");
//         //     }
//         //     EditorGUILayout.EndHorizontal();
//         //     
//         //     EditorGUILayout.HelpBox("123", MessageType.Info);
//         //     
//         //     serializedObject.ApplyModifiedProperties();
//         //     serializedObject.Update();
//         // }
//     }
// }
