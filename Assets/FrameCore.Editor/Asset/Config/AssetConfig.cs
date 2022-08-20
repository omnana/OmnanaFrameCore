using System.Collections.Generic;
using UnityEngine;

namespace FrameCore.Editor
{
    [CreateAssetMenu(fileName = "AssetConfig", menuName = "资源相关/AssetConfig")]
    public class AssetConfig : ScriptableObject
    {
        [Header("规则3：剩下的文件都以最小依赖的规则打包")] 
        [Header("StreamingAssets下")]
        [Header("规则2：StreamingAssetFolder下的文件不参与打包，直接将文件夹复制到")]
        [Header("规则1：IndependentFolder下的文件夹，一个文件夹一个ab")] 
        [SerializeField]  public List<Object> IndependentFolder = new List<Object>();
        [SerializeField] public List<Object> StreamingAssetFolder = new List<Object>();
    }
}
