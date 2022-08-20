using System;
using UnityEngine;

namespace DynamicAtlas
{
    public enum EDynamicAtlasState
    {
        Success,
        NoLeftSpace,
        EmptyArgument,
        NotExist,
        ErrArgument,
    }

    public class PicInfo<TSmallPic>
    {
        public int ID { set; get; }
        public string name { set; get; }

        public IntRect UvRect { get; }
        public Vector4 NormalizeUvRect { get; }
        public TSmallPic Pic { set; get; }

        public PicInfo(int id, string name, IntRect uvRect, Vector2 atlasSize)
        {
            ID = id;
            this.name = name;
            UvRect = uvRect;
            NormalizeUvRect = UvRect.GetNormalUvRect(atlasSize.x, atlasSize.y);
        }
    }

    public interface IDynamicAtlas<TAtlas, TSmallPic> : IDisposable
        where TAtlas : class
        where TSmallPic : class
    {
        EDynamicAtlasState AddPic(TSmallPic smallPic, out int id, out TSmallPic newSmallPic);
        EDynamicAtlasState AddPic(string name, byte[] data, out int id);
        EDynamicAtlasState RemovePic(TSmallPic sprite);
        EDynamicAtlasState RemovePic(int id);

        EDynamicAtlasState GetPic(int id, out PicInfo<TSmallPic> info);

        TAtlas Atlas { get; }
        int AtlasWidth { get; }
        int AtlasHeight { get; }
    }
}