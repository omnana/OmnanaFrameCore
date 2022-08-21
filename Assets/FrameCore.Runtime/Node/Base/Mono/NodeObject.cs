using UnityEngine;

namespace FrameCore.Runtime
{
   [RequireComponent(typeof(StaticNodeCollector))]
   public class NodeObject : MonoBehaviour, IGoGetter
   {
      [SerializeField] public NodeType NodeType;
      [SerializeField] private string m_NodeKey;
      [SerializeField] private ObjectCollector m_objectCollector = new ObjectCollector();

      public int Id => gameObject.GetInstanceID();

      public string NodeKey
      {
         get => m_NodeKey;
         set => m_NodeKey = value;
      }

      public NodeKey Key => _node.Key;

      private BaseNode _node;

      private bool _isInit;

      public T GetGo<T>(string key) where T : Object => m_objectCollector[key] as T;

      public T GetVO<T>() where T : BaseNodeVO
      {
         if (null == _node)
            Load();

         return _node.VO as T;
      }

      public void SetActive(bool isActive) => gameObject.SetActive(isActive);

      private void Load()
      {
         if (_isInit)
            return;
      
         if (string.IsNullOrEmpty(m_NodeKey))
         {
            FrameDebugger.LogError($" [NodeObject] {gameObject.name} nodeKey is null!!!");
            return;
         }

         var key = NodeKeyCollector.GetKey(m_NodeKey);
         if (key == null)
         {
            FrameDebugger.LogError(
               $" [NodeObject] {m_NodeKey} 为静态node，需要在父节点的Builder中的InitStaticNodeKey函数，提前初始化下它的nodeKey!!!");
            return;
         }

         var nodeFactory = key.nodeBuilderDelegate();
         _node = nodeFactory.Build(key);
         nodeFactory.NodeInit(_node, this);
         _isInit = true;
      }

      public void Open(params object[] args)
      {
         if (!gameObject.activeSelf)
         {
            gameObject.SetActive(true);
         }

         Load();
         _node?.Refresh(args);
      }

      #region 生命周期

      private void Awake()
      {
         Load();
         _node?.Init();
      }

      private void OnEnable()
      {
         _node?.Show();
      }

      private void Update()
      {
         _node?.Update();
      }

      private void LateUpdate()
      {
         _node?.LateUpdate();
      }

      private void OnDisable()
      {
         _node?.Hide();
      }

      private void OnDestroy()
      {
         _node?.Stop();
      }

      #endregion
   }
}