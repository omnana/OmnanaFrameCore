using System;
using System.IO;
using System.Security.Cryptography;

namespace FrameCore.Runtime
{
    public static class Md5Helper
    {
        public static string GetMd5HashFromFile(string fileName, int length)
        {
            var buffer = new byte[length];
            var output = new byte[length];
            var str = string.Empty;
            using( Stream inputStream = File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                using (HashAlgorithm hashAlgorithm = new MD5CryptoServiceProvider())
                {
                    var readLength = 0; // 每次读取长度
                    while ((readLength = inputStream.Read(buffer, 0, buffer.Length)) > 0) // 计算MD5
                    {
                        hashAlgorithm.TransformBlock(buffer, 0, readLength, output, 0);
                    }

                    //完成最后计算，必须调用(由于上一部循环已经完成所有运算，所以调用此方法时后面的两个参数都为0)  
                    hashAlgorithm.TransformFinalBlock(buffer, 0, 0);
                    var retVal = hashAlgorithm.Hash;
                    var sb = new System.Text.StringBuilder(32);
                    foreach (var t in retVal)
                    {
                        sb.Append(t.ToString("x2"));
                    }

                    hashAlgorithm.Clear();
                    inputStream.Close();
                    str = sb.ToString();
                };
            }
            return str;
        }
        
        public static string GetSHA1Hash(byte[] bytes)
        {
            return BitConverter.ToString(SHA1.Create().ComputeHash(bytes)).Replace("-", "");
        }
    }
}
