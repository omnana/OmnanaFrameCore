using UnityEngine;

namespace DynamicAtlas.Component
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class Head : MonoBehaviour
    {
        public SpriteRenderer SpriteRenderer;

        private MaterialPropertyBlock _materialPropertyBlock;

        private int _mainTex_ShaderId;
        private int _uvRect_ShaderId;
    
        void Awake()
        {
            SpriteRenderer = GetComponent<SpriteRenderer>();
            _materialPropertyBlock = new MaterialPropertyBlock();
            SpriteRenderer.GetPropertyBlock(_materialPropertyBlock);
            _mainTex_ShaderId = Shader.PropertyToID("_MainTex");
            _uvRect_ShaderId = Shader.PropertyToID("_UVRect");
        }

        public void SetIcon(string picId)
        {
            var picInfo = IdealHeadAtlas.GetPic(picId);
            _materialPropertyBlock.SetTexture(_mainTex_ShaderId, IdealHeadAtlas.Atlas);
            _materialPropertyBlock.SetVector(_uvRect_ShaderId, picInfo.NormalizeUvRect);
            SpriteRenderer.SetPropertyBlock(_materialPropertyBlock);
        }
    }
}
