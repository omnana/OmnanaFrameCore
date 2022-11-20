using UnityEngine;

namespace FrameCore.Util
{
    public static class ByteUtil
    {
        public static byte[] GetBytes(string str)
        {
            return System.Text.Encoding.UTF8.GetBytes(str);
        }

        public static string GetString(byte[] bytes)
        {
            return System.Text.Encoding.UTF8.GetString(bytes);
        }

        public static Sprite ByteToSprite(byte[] datas, int width, int height)
        {
            Texture2D texture = new Texture2D(width, height);
            texture.LoadImage(datas);
            return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one / 2);
        }

        public static byte[] SpriteToByte(Sprite spr)
        {
            return spr.texture.EncodeToPNG();
        }

        public static string BytesToBit(byte[] bytes)
        {
            string strResult = string.Empty;
            for (int i = 0; i < bytes.Length; i++)
            {
                string strTemp = System.Convert.ToString(bytes[i], 2);
                strTemp = strTemp.Insert(0, new string('0', 8 - strTemp.Length));
                strResult += strTemp;
            }

            return strResult;
        }

        public static byte[] BitToBytes(string bit)
        {
            byte[] bytes = new byte[bit.Length / 8];
            for (int i = 0; i < bytes.Length; i++)
                bytes[i] = System.Convert.ToByte(bit.Substring(i * 8, 8), 2);
            return bytes;
        }
    }
}