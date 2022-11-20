using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace FrameCore.Editor
{
    /// <summary>
    /// 设置窗口.
    /// </summary>
    public class SettingWindows :EditorWindow
    {
        private List<MacroItem> macroItemLists = new List<MacroItem>();
 
        private Dictionary<string, bool> dic = new Dictionary<string, bool>();
 
        private string Macro = null;
 
        
        [MenuItem("Omnana/宏设置")]
        public static void Settings()
        {
            SettingWindows sw=EditorWindow.GetWindow<SettingWindows>();//获取指定类型的窗口.
            sw.titleContent = new GUIContent("设置窗口");
            sw.Show();
        }
        
        /// <summary>
        /// 每打开一次窗口就会执行一次
        /// </summary>
        private void OnEnable()
        {
            Macro = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone);//获取宏信息  参数:获取哪个平台下的
            macroItemLists.Clear();//每次打开一次窗口都需要将数据重置.
            // macroItemLists.Add(new MacroItem() { Name = "DEBUG_MODEL", DisplayName = "调试模式", isDebug = true, isRelease = false });
            macroItemLists.Add(new MacroItem() { Name = "DEBUG_LOG", DisplayName = "打印日志", isDebug = true, isRelease = false });
            macroItemLists.Add(new MacroItem() { Name = "STAD_TD", DisplayName = "开启发布", isDebug = false, isRelease = true });
            for (int i = 0; i < macroItemLists.Count; i++)
            {
 
                if (!string.IsNullOrEmpty(Macro) && Macro.IndexOf(macroItemLists[i].Name) != -1)
                {
                    dic[macroItemLists[i].Name] = true;
                }
                else
                {
                    dic[macroItemLists[i].Name] = false;
                }
 
            }
        }
 
        /// <summary>
        /// 绘制窗口条目.
        /// </summary>
        private void OnGUI()
        {
            for (int i = 0; i < macroItemLists.Count; i++)
            {
                EditorGUILayout.BeginHorizontal("box");//开启一个水平行   备注:必须成对出现
                dic[macroItemLists[i].Name] = GUILayout.Toggle(dic[macroItemLists[i].Name], macroItemLists[i].DisplayName);
                EditorGUILayout.EndHorizontal();//结束这个水平行
            }
            EditorGUILayout.BeginHorizontal();//开启一个水平行   备注:必须成对出现
            if (GUILayout.Button("保存", GUILayout.Width(100)))
            {
                SaveMacro();
            }
            // if (GUILayout.Button("调试模式", GUILayout.Width(100)))
            // {
            //     for (int i = 0; i < macroItemLists.Count; i++)
            //     {
            //         dic[macroItemLists[i].Name] = macroItemLists[i].isDebug;
            //     }
            //     SaveMacro();
            // }
            if (GUILayout.Button("发布模式", GUILayout.Width(100)))
            {
                for (int i = 0; i < macroItemLists.Count; i++)
                {
                    dic[macroItemLists[i].Name] = macroItemLists[i].isRelease;
                }
                SaveMacro();
            }
            EditorGUILayout.EndHorizontal();//结束这个水平行
 
        }
        /// <summary>
        /// 保存宏信息
        /// </summary>
        private void SaveMacro()
        {
            Macro = string.Empty;
            foreach (var item in dic)
            {
                if (item.Value)
                {
                    Macro += string.Format("{0};", item.Key);
                }
            }

            PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, Macro);//将信息保存到宏信息里. 参数1:保存到哪个平台  参数2:要保存的内容.
        }
 
        /// <summary>
        /// 宏元素.
        /// </summary>
        public class MacroItem
        {
            /// <summary>
            /// 宏名称
            /// </summary>
            public string Name;
 
            /// <summary>
            /// 窗口上显示的名称
            /// </summary>
            public string DisplayName;
 
            /// <summary>
            /// 是否调试
            /// </summary>
            public bool isDebug;
 
            /// <summary>
            /// 是否发布
            /// </summary>
            public bool isRelease;
        }
    }
}