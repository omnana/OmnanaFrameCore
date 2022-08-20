using FrameCore.Runtime;
using FrameCore.Util;
using UnityEditor;
using UnityEngine;

namespace FrameCore.Editor
{
    public class AbBuildWnd : EditorWindow
    {
        private string _versionCode;
        public string[] options = {"Windows", "Ios", "Android"};
        public BuildTarget[] optionsIndex = {BuildTarget.StandaloneWindows64, BuildTarget.iOS, BuildTarget.Android};
        private int _index;
        private SerializedProperty _rawResourceTarget;
        private string _abFolder;

        [MenuItem("Omnana/资源相关/打整包AB资源")]
        private static void Open()
        {
            var wnd = GetWindow<AbBuildWnd>();
            wnd.OpenWnd(new Vector2(400, 300));
        }

        private void OpenWnd(Vector2 pos)
        {
            titleContent = new GUIContent("打包ab");
            autoRepaintOnSceneChange = true;
            Show();
            _abFolder = GameConfigHelper.GetProduct();
        }

        private void OnGUI()
        {
            _versionCode = EditorGUILayout.TextField("打包资源版本号：", _versionCode);
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("打包的游戏主文件夹:", GUILayout.Width(130));
                EditorGUILayout.LabelField(_abFolder);
                GUILayout.Space(10);
            }
            EditorGUILayout.EndHorizontal();

            var dir = $"{Application.dataPath}/{_abFolder}";
            if (!DirectoryUtil.Exist(dir))
            {
                GUILayout.Space(20);
                EditorGUILayout.HelpBox($"注意：该{_abFolder}文件夹不存在!!", MessageType.Warning);
                return;
            }

            var resFolder = AssetBundleHelper.GetRawResourcesPrefix(_abFolder);
            var resDir = $"{Application.dataPath}/{resFolder}";
            if (!DirectoryUtil.Exist(resDir))
            {
                EditorGUILayout.HelpBox("注意：需要将程序集所使用的的资源文件夹放在与程序集同目录下，资源文件夹名称同一为RawResources", MessageType.Warning);
                return;
            }

            GUILayout.Space(20);
            if (GUILayout.Button("打我", GUILayout.Width(200)))
            {
                // // 用这个方式拷贝dll，避免直接拷贝dll，没有正确使用相应平台的编译开关
                // DllTool.CompileDllWin64();
                //
                // DllTool.CopyDll2Resource(GameConfigEditorHelper.ProductName, dir + "/RawResources/Dll");
                
                // 现在只是window平台，所以直接拷贝
                DllTool.CopyScriptAssembliesDll2Resource(GameConfigHelper.GetProduct(), dir + "/RawResources/Dll");

                // 打包资源
                AbTool.BuildCustomAllAssetBundles(_abFolder);
            }
        }
    }
}
