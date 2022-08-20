using System;
using System.IO;
using UnityEngine;

namespace FrameCore.Util
{
    public static class DirectoryUtil
    {
        public static string GetProjectDirectory(string dir)
        {
            return $"{Environment.CurrentDirectory.Replace("\\", "/")}/{dir}";
        }
        
        public static bool Exist(string dir)
        {
            return Directory.Exists(dir);
        }

        public static void CreateDirectory(string dir)
        {
            Directory.CreateDirectory(dir);
        }

        public static bool IsEmpty(string dir)
        {
            var info = new DirectoryInfo(dir);
            return info.GetFiles().Length != 0;
        }

        public static void DeleteDirectory(string dir)
        {
            if (!Exist(dir))
                return;

            var direction = new DirectoryInfo(dir);
            var files = direction.GetFiles("*", SearchOption.AllDirectories);
            foreach (var fileInfo in files)
            {
                var filePath = fileInfo.FullName;
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }

                DeleteDirectory(fileInfo.FullName);
            }
        }

        public static void CopyDirectory(string sourceDirectory, string targetDirectory, string filter = "")
        {
            try
            {
                if (!Directory.Exists(targetDirectory))
                {
                    Directory.CreateDirectory(targetDirectory);
                }

                var dir = new DirectoryInfo(sourceDirectory);
                var filings = dir.GetFileSystemInfos();
                foreach (var i in filings)
                {
                    if (i is DirectoryInfo) //判断是否文件夹
                    {
                        if (!Directory.Exists(targetDirectory + "\\" + i.Name))
                        {
                            //目标目录下不存在此文件夹即创建子文件夹
                            Directory.CreateDirectory(targetDirectory + "\\" + i.Name);
                        }

                        //递归调用复制子文件夹
                        CopyDirectory(i.FullName, targetDirectory + "\\" + i.Name);
                    }
                    else
                    {
                        if (!i.FullName.EndsWith(filter) || string.IsNullOrEmpty(filter))
                            //不是文件夹即复制文件，true表示可以覆盖同名文件
                            File.Copy(i.FullName, targetDirectory + "\\" + i.Name, true);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogErrorFormat("复制文件出现异常 : {0}", ex.Message);
            }
        }

        // 遍历文件夹下本级文件，再接着往下遍历文件夹
        public static void TraverseFileOnFolder(string dir, Action<FileInfo> onSearch)
        {
            var directoryInfo = new DirectoryInfo(dir);
            var files = directoryInfo.GetFiles();
            foreach (var fileInfo in files)
            {
                onSearch?.Invoke(fileInfo);
            }

            var dirs = directoryInfo.GetDirectories();
            foreach (var d in dirs)
            {
                TraverseFileOnFolder(d.FullName, onSearch);
            }
        }

        /// <summary>
        /// 删除掉空文件夹
        /// 所有没有子“文件系统”的都将被删除
        /// </summary>
        /// <param name="folder"></param>
        public static void KillEmptyDirectory(string folder)
        {
            var dir = new DirectoryInfo(folder);
            var subDirs = dir.GetDirectories("*.*", SearchOption.AllDirectories);
            foreach (var subDir in subDirs)
            {
                var subFiles = subDir.GetFileSystemInfos();
                if (subFiles.Length == 0)
                {
                    subDir.Delete();
                }
            }
        }
    }
}
