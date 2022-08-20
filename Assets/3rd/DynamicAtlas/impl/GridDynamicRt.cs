using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;

namespace DynamicAtlas
{
    public class GridDynamicRt : IDynamicAtlas<RenderTexture, byte[]>
    {
        class ChildPicInfo
        {
            public int ID { get; }
            public string Name { get; set; }

            public IntRect Rect { get; }

            public ChildPicInfo(int id, IntRect r)
            {
                ID = id;
                Rect = r;
            }

            public void SetEmpty()
            {
                Name = string.Empty;
            }

            public override string ToString()
            {
                return $"x={Rect.x} y = {Rect.y} id={ID}";
            }

            public bool IsEmpty => string.IsNullOrEmpty(Name);
        }

        private RenderTexture _rt;
        private int _atlasWidth;
        private int _atlasHeight;
        private int _cellWidth;
        private int _cellHeight;
        private ComputeShader _cShader;
        private ComputeBuffer _cBuffer;
        private int _bufferByteSize = 0;
        private int _kernelIndex;
        private int shaderProperID_rt;
        private int shaderProperID_colorData;
        private int shaderProperID_startX;
        private int shaderProperID_startY;
        private int shaderProperID_width;
        private int shaderProperID_setEmpty;
        private Dictionary<int, int> _dicId2ListIndex; //将id和list中的index映射起来,方便查找
        private List<ChildPicInfo> _list;
        private Stack<int> _stackEmptyChildPicID; //记录未使用的childPic的id,避免遍历List

        public GridDynamicRt(ComputeShader computeShader, int atlasWidth, int atlasHeight, int cellWidth, int cellHeight)
        {
            if (cellWidth >= atlasWidth / 2)
            {
                throw new ArgumentException("[GridDynamicRt]:cellWidth >= atlasWidth / 2");
            }

            if (cellHeight >= atlasHeight / 2)
            {
                throw new ArgumentException("[GridDynamicRt]:cellHeight >= atlasHeight / 2");
            }


            _atlasWidth = atlasWidth;
            _atlasHeight = atlasHeight;
            _cellWidth = cellWidth;
            _cellHeight = cellHeight;
            _cShader = computeShader;
            Init();
        }

        void Init()
        {
            _bufferByteSize = _cellHeight * _cellWidth * 4; //1个像素4字节
            _kernelIndex = _cShader.FindKernel("CSMain");
            _cBuffer = new ComputeBuffer(_bufferByteSize, 4, ComputeBufferType.Structured);
            _rt = new RenderTexture(new RenderTextureDescriptor()
            {
                width = _atlasWidth,
                height = _atlasHeight,
                autoGenerateMips = false,
                colorFormat = RenderTextureFormat.ARGB32,
                dimension = TextureDimension.Tex2D,
                memoryless = RenderTextureMemoryless.None,
                graphicsFormat = GraphicsFormat.R8G8B8A8_UNorm,
                depthBufferBits = 0,
                enableRandomWrite = true,
                useMipMap = false,
                msaaSamples = 1,
                stencilFormat = GraphicsFormat.None,
                vrUsage = VRTextureUsage.None,
                useDynamicScale = false,
                mipCount = 0,
                shadowSamplingMode = ShadowSamplingMode.None,
                volumeDepth = 1,
            });
            _rt.Create(); //直接激活掉
            shaderProperID_rt = Shader.PropertyToID("_rt");
            shaderProperID_colorData = Shader.PropertyToID("_colorData");
            shaderProperID_startX = Shader.PropertyToID("startX");
            shaderProperID_startY = Shader.PropertyToID("startY");
            shaderProperID_setEmpty = Shader.PropertyToID("setEmpty");
            shaderProperID_width = Shader.PropertyToID("width");

            _cShader.SetTexture(_kernelIndex, shaderProperID_rt, _rt);
            _cShader.SetBuffer(_kernelIndex, shaderProperID_colorData, _cBuffer);
            var row = Mathf.CeilToInt(_atlasWidth / _cellWidth);
            var col = Mathf.CeilToInt(_atlasHeight / _cellHeight);
            _list = new List<ChildPicInfo>(row * col);
            _dicId2ListIndex = new Dictionary<int, int>(row * col * 2);
            _stackEmptyChildPicID = new Stack<int>(row * col);
            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < col; j++)
                {
                    var id = i * row + j;
                    var rect = new IntRect(j * _cellWidth, i * _cellHeight, _cellWidth, _cellHeight);
                    var info = new ChildPicInfo(id, rect);
                    _stackEmptyChildPicID.Push(id);
                    _list.Add(info);
                    _dicId2ListIndex.Add(id, _list.Count - 1);
                }
            }
        }

        public void Dispose()
        {
            _cBuffer.Dispose();
        }

        public EDynamicAtlasState AddPic(byte[] smallPic, out int id, out byte[] newSmallPic)
        {
            throw new NotImplementedException("[GridDynamicRt]:不做实现, 返回数据富文本浪费内存,无意义");
        }

        public EDynamicAtlasState AddPic(string name, byte[] data, out int id)
        {
            if (data == null)
            {
                Debug.LogError($"[GridDynamicRt]:data是空值");
                id = -1;
                return EDynamicAtlasState.EmptyArgument;
            }

            if (data.Length != _bufferByteSize)
            {
                Debug.LogError($"[GridDynamicRt]:数据长度不够({data.Length}) 需要的长度={_bufferByteSize}");
                id = -1;
                return EDynamicAtlasState.ErrArgument;
            }

            if (FindAndCostFirstEmpty(out var info, name))
            {
                id = info.ID;
                _cShader.SetInt(shaderProperID_startX, info.Rect.x);
                _cShader.SetInt(shaderProperID_startY, info.Rect.y);
                _cShader.SetInt(shaderProperID_width, info.Rect.width);
                _cShader.SetBool(shaderProperID_setEmpty, false);
                _cBuffer.SetData(data, 0, 0, _bufferByteSize);
                Dispatch();
                return EDynamicAtlasState.Success; 
            }
            else
            {
                id = -1;
                return EDynamicAtlasState.NoLeftSpace;
            }
        }

        void Dispatch()
        {
            _cShader.Dispatch(_kernelIndex, _cellWidth / 8, _cellHeight / 8, 1);
        }

        /// <summary>
        /// 获取并从记录了empty的栈中弹出
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        bool FindAndCostFirstEmpty(out ChildPicInfo info, string name)
        {
            info = null;
            if (_stackEmptyChildPicID.Count == 0)
            {
                return false;
            }

            var id = _stackEmptyChildPicID.Pop();

            if (FindInfo(id, out var childPicInfo))
            {
                info = childPicInfo;
                info.Name = name;
                return true;
            }
            else
            {
                Debug.LogError("[GridDynamicRt]:FindAndCostFirstEmpty 内部逻辑错误,这是个bug, stack里边有未使用的id,当dic里边却没有!!");
                return false;
            }
        }


        // bool FindAndCostFirstEmpty(out ChildPicInfo info, string name)
        // {
        //     for (int i = 0; i < _list.Count; i++)
        //     {
        //         var p = _list[i];
        //         if (p.IsEmpty)
        //         {
        //             info = p;
        //             info.Name = name;
        //
        //             return true;
        //         }
        //     }
        //
        //     info = null;
        //     return false;
        // }


        bool FindInfo(int id, out ChildPicInfo info)
        {
            if (_dicId2ListIndex.TryGetValue(id, out var index))
            {
                info = _list[index];
                return true;
            }
            else
            {
                info = null;
                return false;
            }
        }

        public EDynamicAtlasState RemovePic(byte[] sprite)
        {
            throw new NotImplementedException("[GridDynamicRt]:不做实现,在cpu侧缓存数据浪费内存");
        }

        public EDynamicAtlasState RemovePic(int id)
        {
            if (FindInfo(id, out var info))
            {
                _stackEmptyChildPicID.Push(id);
                info.SetEmpty();
                _cShader.SetBool(shaderProperID_setEmpty, true);
                _cShader.SetInt(shaderProperID_startX, info.Rect.x);
                _cShader.SetInt(shaderProperID_startY, info.Rect.y);
                _cShader.SetInt(shaderProperID_width, info.Rect.width);
                Dispatch();
                return EDynamicAtlasState.Success;
            }
            else
            {
                Debug.LogError("[GridDynamicRt]:不存在的图片id={id}");
                return EDynamicAtlasState.NotExist;
            }
        }

        public EDynamicAtlasState GetPic(int id, out PicInfo<byte[]> info)
        {
            if (FindInfo(id, out var childPicInfo))
            {
                info = new PicInfo<byte[]>(childPicInfo.ID, childPicInfo.Name, childPicInfo.Rect, new Vector2(_atlasWidth, _atlasHeight));
                return EDynamicAtlasState.Success;
            }
            else
            {
                info = null;
                return EDynamicAtlasState.NotExist;
            }
        }

        public RenderTexture Atlas => _rt;
        public int AtlasWidth => _atlasWidth;
        public int AtlasHeight => _atlasHeight;
    }
}