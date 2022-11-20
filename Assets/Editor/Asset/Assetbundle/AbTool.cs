using FrameCore.Util;
using UnityEditor;
using UnityEngine;
using System.IO;
using System;
using FrameCore.Runtime;

namespace FrameCore.Editor
{
    public static class AbTool
    {
        private static readonly AbBuilder AbBuilder = new AbBuilder();
        
        // 进行自定义游戏打包，仅针对win平台a
        public static void BuildCustomAllAssetBundles()
        {
            string productName = GameConfigHelper.GetProduct();
            var config = IdealResource.Load<AssetConfig>($"Config\\AssetConfig\\{productName}.asset");
            if (config == null)
            {
                Debug.LogWarning($"请在Resources文件夹下创建 {productName}的AssetConfig配置！！" +
                                 $"右键Config/AssetConfig文件夹（没有这个文件夹，就创建一个），[create/资源管理/AssetConfig]，请将文件命名为 {productName} ");
                return;
            }

            var outputFolder = AssetBundleHelper.GetOutputFolderEditor();
            DirectoryUtil.DeleteDirectory(outputFolder);
            DirectoryUtil.CreateDirectory(outputFolder);
            // if (!DirectoryUtil.Exist(outputFolder))
            // {
            //     DirectoryUtil.CreateDirectory(builderFolder);
            // }

            try
            {
                var builds = AbBuilder.Analyze(config);

                // // 删除不用文件
                // var deleteList = AbBuilder.DeleteList;
                // if (deleteList != null)
                // {
                //     foreach (var file in deleteList)
                //     {
                //         var filePath = $"{builderFolder}/{file}";
                //         var manifestFilePath = $"{builderFolder}/{file}.manifest";
                //         FileUtility.DeleteAllFile(filePath);
                //         FileUtility.DeleteAllFile(manifestFilePath);
                //     }
                // }

                BuildPipeline.BuildAssetBundles(outputFolder, builds, BuildAssetBundleOptions.ChunkBasedCompression, BuildTarget.StandaloneWindows64);

                // // 删除空文件夹
                // DirectoryUtil.KillEmptyDirectory(streamAssetFolder);

                Debug.Log($"[{productName}] 完成ab打包。输出路径：{outputFolder}");
                System.Diagnostics.Process.Start(outputFolder);
            }
            catch (Exception e)
            {
                Debug.LogError($"打包ab Error : {e.Message}");
            }
        }

        // 生成Resources配置
        public static void GenerateResourceConfig()
        {
            var path = Application.dataPath + "/Resources/";
            var files = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);
            var txt = string.Empty;
            foreach (var file in files)
            {
                if (file.EndsWith(".meta"))
                    continue;

                var fullPath = Path.GetFullPath(file);
                var name = fullPath.Substring(fullPath.IndexOf(@"Resources\") + @"Resources\".Length).Replace("\\", "/").ToLower();
                txt += name + "|";
            }

            if(!string.IsNullOrEmpty(txt))
                txt = txt.Remove(txt.LastIndexOf("|"));
            var filePath = $"{path}FileList.bytes";
            if (!File.Exists(filePath))
                File.Create(filePath).Dispose();

            var bytes = ByteUtil.GetBytes(txt);
            File.WriteAllBytes(filePath, bytes);
        }

        // /// <summary>
        // /// 生成版本资源列表
        // /// </summary>
        // /// <param name="buildTarget"></param>
        // public static void BuildAllAssetBundlesConfigList(BuildTarget buildTarget, string versionCode)
        // {
        //     var platformName = buildTarget;
        //     var fullPath = Application.streamingAssetsPath + "/" + platformName;
        //     if (Directory.Exists(fullPath))
        //     {
        //         var direction = new DirectoryInfo(fullPath);
        //         var files = direction.GetFiles("*", SearchOption.AllDirectories);
        //         var buffer = new byte[16 * 1024]; // 一次读取长度 16384 = 16 * 1024 kb
        //         var output = new byte[buffer.Length];
        //         var fileVersionDatas = new List<FileVersionData>();
        //         var sb = new System.Text.StringBuilder(32);
        //         for (int i = 0; i < files.Length; i++)
        //         {
        //             if (files[i].Name.EndsWith(".meta"))
        //             {
        //                 continue;
        //             }
        //
        //             sb.Clear();
        //             var filePath = files[i].ToString();
        //             var inputStream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        //             var size = (int) inputStream.Length;
        //             var realName = files[i].Name;
        //             if (!files[i].Directory.Name.Equals(platformName))
        //             {
        //                 realName = files[i].Directory.Name + "/" + files[i].Name;
        //             }
        //
        //             fileVersionDatas.Add(new FileVersionData()
        //             {
        //                 Name = realName,
        //                 Size = size,
        //                 Md5 = sb.ToString(),
        //                 Version = int.Parse(versionCode),
        //             });
        //         }
        //
        //         var versionListPath = AssetBundlePathHelper.GetAssetBundlePath() + "/" + versionCode + ".txt";
        //         sb.Clear();
        //         for (var i = 0; i < fileVersionDatas.Count; i++)
        //         {
        //             sb.AppendLine(fileVersionDatas[i].ToString());
        //         }
        //         
        //         if (!File.Exists(versionListPath)) 
        //             File.Create(versionListPath);
        //         File.WriteAllText(versionListPath, sb.ToString());
        //     }
        // }
    }
}
