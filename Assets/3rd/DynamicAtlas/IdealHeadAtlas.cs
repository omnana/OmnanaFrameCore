using System.Collections.Generic;
using DynamicAtlas.Config;
using DynamicAtlas.Util;
using UnityEngine;

namespace DynamicAtlas
{
    // 头像大图集，目前只支持单元宽高相等的图集
    public static class IdealHeadAtlas
    {
        public class IdealHeadAtlasInfo
        {
            public int width;
            public int height;
            public int cellWidth;
            public int cellHeight;
        }
        
        private static GridDynamicRt _atlas;
        private static Dictionary<string, int> _picDic;
        public static RenderTexture Atlas { get; private set; }

        public static bool IsInit { get; private set; }
        public static IdealHeadAtlasInfo RectInfo { get; private set; }

        private static Texture2D _tool;
        private static byte[] _bytes;

        // 只能在EntranceCore内初始化
        public static void Init(LoadAssetHandle loadAssetHandle)
        {
            _picDic = new Dictionary<string, int>(4096);
            DynamicAtlasUtil.LoadAssetHandle = loadAssetHandle;
            var configSo = DynamicAtlasUtil.LoadAssetHandle("Config/DynamicAtlasConfig.asset");
            if (configSo == null)
            {
                Debug.LogError(
                    $"无法读取到DynamicAtlasConfig，右键选择创建DynamicAtlasConfig");
                return;
            }

            var config = configSo as DynamicAtlasConfig;
            var cs = DynamicAtlasUtil.LoadAssetHandle("ComputeShader/RtAtlasCpShader.compute") as ComputeShader;
            RectInfo = new IdealHeadAtlasInfo
            {
                width = config.AtlasWidth,
                height = config.AtlasHeight,
                cellWidth = config.CellWidth,
                cellHeight = config.CellHeight,
            };
            _tool = new Texture2D(RectInfo.cellWidth, RectInfo.cellHeight);
            _bytes = new byte[RectInfo.cellWidth * RectInfo.cellHeight * 4];
            _atlas = new GridDynamicRt(cs, RectInfo.width, RectInfo.height, RectInfo.cellWidth, RectInfo.cellHeight);
            Atlas = _atlas.Atlas;
            Debug.Log($"初始化头像大图集");
            IsInit = true;
        }

        // 将下载下来的头像导入图集中
        public static void AddPic(string texName, byte[] bytes)
        {
            if (_atlas == null)
            {
                Debug.LogError($"GridDynamicRt还未初始化！！");
                return;
            }

            if (_picDic.ContainsKey(texName))
            {
                Debug.LogError($"图片{texName}已存在！！！");
                return;
            }

            // 先这么转
            _tool.LoadImage(bytes);
            DynamicAtlasUtil.Scale(_tool, _bytes, RectInfo.cellHeight, RectInfo.cellHeight);
            _atlas.AddPic(texName, _bytes, out var id);
            _picDic.Add(texName, id);
        }

        // 将头像导入图集中, 不通过_tool
        public static void AddPicImmediate(string texName, byte[] bytes)
        {
            if (_atlas == null)
            {
                Debug.LogError($"GridDynamicRt还未初始化！！");
                return;
            }

            if (_picDic.ContainsKey(texName))
            {
                Debug.LogError($"图片{texName}已存在！！！");
                return;
            }

            _atlas.AddPic(texName, bytes, out var id);
            _picDic.Add(texName, id);
        }

        public static bool Contains(string texName)
        {
            return _picDic.ContainsKey(texName);
        }

        public static PicInfo<byte[]> GetPic(string texName)
        {
            if (_atlas == null)
            {
                Debug.LogError($"GridDynamicRt还未初始化！！");
                return null;
            }

            if (!_picDic.ContainsKey(texName))
            {
                Debug.LogWarning($"图片{texName}不存在！！！");
                return null;
            }

            var ret = _atlas.GetPic(_picDic[texName], out var info);
            if (ret != EDynamicAtlasState.Success)
            {
                Debug.LogError($"error : {ret}！！");
                return null;
            }

            return info;
        }

        public static void RemovePic(string texName)
        {
            if (_atlas == null)
            {
                Debug.LogError($"GridDynamicRt还未初始化！！");
                return;
            }

            if (_picDic.ContainsKey(texName))
            {
                var id = _picDic[texName];
                _atlas.RemovePic(id);
                _picDic.Remove(texName);
            }
        }

        public static Vector4 NormalUvRect(IntRect uvRect)
        {
            var x = uvRect.x / (float) RectInfo.width;
            var y = uvRect.y / (float) RectInfo.height;
            var z = x + RectInfo.cellWidth / (float) RectInfo.width;
            var w = y + RectInfo.cellHeight / (float) RectInfo.width;
            return new Vector4(x, y, z, w);
        }

        public static void Dispose()
        {
            if (_atlas == null)
            {
                Debug.LogError($"GridDynamicRt还未初始化！！");
                return;
            }

            _atlas.Dispose();
            _picDic.Clear();
            IsInit = false;
        }

        public static void Clear()
        {
            if (!IsInit)
            {
                Debug.LogError($"GridDynamicRt还未初始化！！");
                return;
            }

            foreach (var id in _picDic.Values)
            {
                _atlas.RemovePic(id);
            }
            
            _picDic.Clear();
        }
    }
}
