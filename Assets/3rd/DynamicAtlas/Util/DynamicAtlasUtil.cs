using UnityEngine;

namespace DynamicAtlas.Util
{
    public delegate Object LoadAssetHandle(string assetName);
    public class DynamicAtlasUtil
    {     
        public static LoadAssetHandle LoadAssetHandle { get; set; }
        
        /// <summary>
        /// 直白的转换方法
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="bytes"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public static void Scale(Texture2D texture, byte[] bytes, int width, int height)
        {
            for (int px = 0, pixelCnt = width * height; px < pixelCnt; px++)
            {
                var col = texture.GetPixelBilinear((float) px % width / width,
                    Mathf.Floor((float) px / height) / height);
                var idx = px * 4;
                bytes[idx] = (byte) (col.r * 255f);
                bytes[idx + 1] = (byte) (col.g * 255f);
                bytes[idx + 2] = (byte) (col.b * 255f);
                bytes[idx + 3] = (byte) (col.a * 255f);
            }
        } 
    }
}
