using UnityEngine;
using UnityEngine.UI;

namespace FrameCore.Runtime
{
    [RequireComponent(typeof(CanvasRenderer))]
    public class UIMesh : Graphic
    {
        [SerializeField] private bool showAnimation;
        [SerializeField] private MeshFilter mMeshFilter;
        [SerializeField] private MeshRenderer mMeshRenderer;

        private Mesh _mesh;

        public override Texture mainTexture => !mMeshRenderer ? s_WhiteTexture :
            Application.isPlaying ? mMeshRenderer.material.mainTexture : mMeshRenderer.sharedMaterial.mainTexture;

        private Mesh TargetMesh => Application.isPlaying ? mMeshFilter.mesh : mMeshFilter.sharedMesh;

        protected override void Awake()
        {
            base.Awake();
            _mesh = new Mesh();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            _mesh.Clear();
            _mesh = null;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            if(showAnimation)
                Canvas.willRenderCanvases += OnCanvasRender;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            if(showAnimation)
                Canvas.willRenderCanvases -= OnCanvasRender;
        }

        protected override void UpdateGeometry()
        {
            if (!mMeshFilter || !mMeshRenderer)
            {
                Debug.LogWarning("请在属性面板上添加mesh相关引用！！");
                return;
            }
            
            base.UpdateGeometry();
            UpdateMesh();
        }

        private void UpdateMesh()
        {
            var newMesh = TargetMesh;
            var vers = newMesh.vertices;
            for (int i = 0; i < vers.Length; i++)
            {
                vers[i] *= rectTransform.rect.size;
            }

            _mesh.Clear();
            _mesh.SetVertices(vers);
            _mesh.SetTriangles(newMesh.triangles, 0);
            _mesh.SetUVs(0, newMesh.uv);
            _mesh.SetNormals(newMesh.normals);
            _mesh.SetTangents(newMesh.tangents);
            _mesh.SetColors(_mesh.colors);
            canvasRenderer.SetMesh(_mesh);
        }

        private void OnCanvasRender()
        {
            if (!mMeshFilter || !mMeshRenderer)
            {
                return;
            }

            UpdateMesh();
        }
    }
}
