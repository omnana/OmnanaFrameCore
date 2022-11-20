using UnityEditor;
using UnityEngine;

namespace FrameCore.Editor
{
    public class AbDependenceWnd : EditorWindow
    {
        private static string[] _files;
        
        [MenuItem("Assets/检查选中资源的依赖项")]
        public static void Open()
        {
            var wnd = GetWindow<AbDependenceWnd>();
            wnd.OpenWnd(new Vector2(400, 300));
        }

        private void OpenWnd(Vector2 pos)
        {
            titleContent = new GUIContent("检查资源依赖");
            autoRepaintOnSceneChange = true;
            Show();
            position = new Rect(pos.x, pos.x, 500, 500);
            // _files = Directory.GetFiles(AbFileHelper.RawResourcesPath, "*.*", SearchOption.AllDirectories);
        }

        private void OnGUI()
        {
            if (Selection.activeGameObject == null) return;

            var objName = Selection.activeGameObject.name;
            foreach (var file in _files)
            {
                if (file.EndsWith(".meta") || !file.Contains(objName))
                    continue;

                //提取在unity资源Assets目录下路径
                string unityPath = file.Substring(file.IndexOf("Assets/"));
                unityPath = unityPath.Replace("\\", "/");
                var dps = AssetDatabase.GetDependencies(unityPath);
                if (dps == null)
                    continue;
                
                foreach(var d in dps)
                {
                    if (d.EndsWith(".cs")) continue;
                    EditorGUILayout.TextField(d);
                }
            }
        }
    }
}
