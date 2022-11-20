using System;
using System.IO;

namespace FrameCore.Utility
{
    public static class FileUtility
    {
        public static byte[] ReadFileToByte(string path)
        {
            FileStream fileStream = new FileStream(path, FileMode.Open);
            byte[] bytes = new byte[fileStream.Length];
            fileStream.Read(bytes, 0, bytes.Length);
            fileStream.Close();
            return bytes;
        }

        public static string ReadFileToText(string path) => File.ReadAllText(path);

        /// <summary>
        /// 搜索文件夹下所有的文件，包括子文件下的
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="condition"></param>
        /// <param name="onAction"></param>
        public static void SearchAllFile(string folder, string condition, Action<string, string> onAction)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(folder);
            foreach (var fileInfo in directoryInfo.GetFiles())
            {
                if (fileInfo.Name.EndsWith(condition))
                {
                    onAction?.Invoke(Path.GetFileNameWithoutExtension(fileInfo.Name), fileInfo.FullName);
                }
            }

            DirectoryInfo[] directoryInfos = directoryInfo.GetDirectories();
            foreach (DirectoryInfo f in directoryInfos)
            {
                SearchAllFile(f.FullName, condition, onAction);
            }
        }

        public static string SearchFileByType(string path, Type type)
        {
            string[] files = Directory.GetFiles(path, ".", SearchOption.AllDirectories);
            for (int i = 0; i < files.Length; i++)
            {
                if (!files[i].Contains(type.Name + ".cs"))
                    continue;

                return files[i].Replace("/", "\\");
            }

            return null;
        }

        public static void SaveFile(string savePath, string file)
        {
            if (File.Exists(savePath))
            {
                File.Delete(savePath);
            }

            File.Move(file, savePath);
        }

        public static void DeleteAllFile(string fullPath)
        {
            if (!File.Exists(fullPath))
                return;

            File.Delete(fullPath);
        }

        public static bool Exists(string path)
        {
            return File.Exists(path);
        }

        public static void CopyFile(string target, string dest)
        {
            File.Copy(target, dest);
        }

        public static string GetFilePathWithoutExtension(string filePath)
        {
            var length = filePath.LastIndexOf(".");
            if (length == -1)
                return filePath;
            return filePath.Substring(0, length);
        }

        public static void WriteFile(string savePath, string content)
        {
            if (!File.Exists(savePath))
            {
                File.Create(savePath).Dispose();
            }
            File.WriteAllText(savePath, content);
        }

        public static void WriteFile(string savePath, byte[] bytes)
        {
            if (!File.Exists(savePath))
            {
                File.Create(savePath).Dispose();
            }
            File.WriteAllBytes(savePath, bytes);
        }
    }
}