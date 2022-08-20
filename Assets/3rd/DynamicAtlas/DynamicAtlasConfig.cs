using UnityEngine;

namespace DynamicAtlas.Config
{
    [CreateAssetMenu(fileName = "DynamicAtlasConfig", menuName = "Create DynamicAtlasConfig", order = 0)]
    public class DynamicAtlasConfig : ScriptableObject
    {
        [Header("大图集宽度")]
        public int AtlasWidth = 4096;
        [Header("大图集高度")]
        public int AtlasHeight = 4096;
        [Header("图集单元宽度")]
        public int CellWidth = 72;
        [Header("图集单元高度")]
        public int CellHeight = 72;
    }
}