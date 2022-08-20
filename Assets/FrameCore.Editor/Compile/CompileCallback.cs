namespace FrameCore.Editor
{

// #if UNITY_EDITOR
//     public class CompileCallback
//     {
//         [UnityEditor.Callbacks.DidReloadScripts]
//         private static void OnScriptsReloaded()
//         {
//             var gameConfig = Persistence.Read(new PersistenceKey(nameof(GameConfig)), new GameConfig());
//             var product = gameConfig.ProductName;
//             if (string.IsNullOrEmpty(product))
//             {
//                 Debug.LogWarning($"请到Tools/游戏配置/填写游戏程序集名称");
//                 return;
//             }
//
//             try
//             {
//                 var dllPath = Environment.CurrentDirectory + $"\\Library\\ScriptAssemblies\\{product}.dll";
//                 var destPathFile = Application.dataPath + $"/{product}/RawResources/Dll/{product}.dll.bytes";
//             
//                 if (!File.Exists(dllPath)) 
//                     return;
//             
//                 var bytes = File.ReadAllBytes(dllPath);
//
//                 if (!File.Exists(destPathFile))
//                 {
//                     File.Create(destPathFile).Dispose();
//                 }
//             
//                 File.WriteAllBytes(destPathFile, bytes);
//
//             }
//             catch (Exception e)
//             {
//                 Debug.LogError(e.Message);
//             }
//         }
//     }
// #endif
}