using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using TableTool.Editor;
using UnityEditor;
using UnityEngine;

namespace DataTable.Editor
{
    public class DataTableWindow : EditorWindow
    {
        class ChooseDataTable
        {
            public string name;
            public string filePath;
            public bool choose;
        }

        private List<ChooseDataTable> _csvFiles = new List<ChooseDataTable>();

        private Vector2 _scrollPos;
        private bool _init;

        // private DataTableConfigSo _configSo;
        private string csvKey = "csv";
        private string scriptKey = "script";

        [MenuItem("Tools/配置表/解析配置，生成对应脚本", false, 1)]
        public static void OpenWindow()
        {
            //创建窗口
            Rect wr = new Rect(0, 0, 400, 600);
            DataTableWindow window =
                (DataTableWindow) GetWindowWithRect(typeof(DataTableWindow), wr, true, "DataTableWindow");
            window.Show();
        }
        //
        // [MenuItem("Tools/Rename", false, 1)]
        // public static void ReName()
        // {
        //     string dir = Application.dataPath + "/PreResources/Sprite/AI";
        //     var dirInfo = new DirectoryInfo(dir);
        //     var files = new List<string>();
        //     var metaFiles = new List<string>();
        //     foreach (var file in dirInfo.GetFiles())
        //     {
        //         if (!file.Name.EndsWith(".meta"))
        //         {
        //             files.Add(file.FullName);
        //         }
        //         else
        //         {
        //             metaFiles.Add(file.FullName);
        //         }
        //     }
        //
        //     for (var index = 0; index < files.Count; index++)
        //     {
        //         var sourceFile = files[index];
        //         var dirPrefix = sourceFile.Substring(0, sourceFile.LastIndexOf("\\"));
        //         var destFile = dirPrefix + "\\" + index + ".png";
        //         File.Move(sourceFile, destFile);
        //     }
        //
        //     for (var index = 0; index < metaFiles.Count; index++)
        //     {
        //         var sourceFile = metaFiles[index];
        //         var dirPrefix = sourceFile.Substring(0, sourceFile.LastIndexOf("\\"));
        //         var destFile = dirPrefix + "\\" + index + ".png.meta";
        //         File.Move(sourceFile, destFile);
        //     }
        // }


        private void OnEnable()
        {
            _init = true;
            RefreshView();
        }

        private void RefreshView()
        {
            var csvFolder = EditorPrefs.GetString(csvKey, "");
            if (!string.IsNullOrEmpty(EditorPrefs.GetString(csvKey, "")))
            {
                var files = DataTableUtil.GetCsvFiles(csvFolder);
                _csvFiles = new List<ChooseDataTable>();
                foreach (var file in files)
                {
                    _csvFiles.Add(new ChooseDataTable()
                    {
                        name = Path.GetFileNameWithoutExtension(file),
                        filePath = file
                    });
                }
            }
        }

        private void OnGUI()
        {
            if (!_init)
            {
                EditorGUILayout.HelpBox("请在Resources/Config文件夹下创建DataTableConfig.asset文件!!", MessageType.Warning);
                return;
            }

            // csv路径
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("csv主文件夹", GUILayout.Width(100));
            var csvFolder = EditorPrefs.GetString(csvKey, "");
            GUI.enabled = false;
            {
                EditorGUILayout.TextField(csvFolder);
            }
            GUI.enabled = true;
            if (GUILayout.Button("选择"))
            {
                EditorPrefs.GetString(csvKey, "");
                var folder = EditorUtility.OpenFolderPanel("select path", "", "");
                EditorPrefs.SetString(csvKey, folder);
            }

            EditorGUILayout.EndHorizontal();

            // 输出路径
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("脚本主文件夹", GUILayout.Width(100));
            var scriptFolder = EditorPrefs.GetString(scriptKey, "");
            GUI.enabled = false;
            {
                EditorGUILayout.TextField(scriptFolder);
            }
            GUI.enabled = true;
            if (GUILayout.Button("选择"))
            {
                EditorPrefs.GetString(scriptKey, "");
                scriptFolder = EditorUtility.OpenFolderPanel("select path", "", "");
                EditorPrefs.SetString(scriptKey, scriptFolder);
            }

            EditorGUILayout.EndHorizontal();

            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos, GUILayout.Width(400), GUILayout.Height(400));
            foreach (var t in _csvFiles)
            {
                t.choose = GUILayout.Toggle(t.choose, t.name);
            }

            EditorGUILayout.EndScrollView();
            EditorGUILayout.Space(10);

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("解析", GUILayout.Width(100)))
            {
                var files = _csvFiles.FindAll(f => f.choose);
                if (files.Count > 0)
                {
                    foreach (var file in files)
                    {
                        DataTableWriter.WriteTable(file.filePath, scriptFolder);
                    }

                    Debug.Log("解析csv，生成脚本完成");
                }
            }

            if (GUILayout.Button("刷新", GUILayout.Width(100)))
            {
                RefreshView();
            }

            EditorGUILayout.EndHorizontal();
        }
    }
}
