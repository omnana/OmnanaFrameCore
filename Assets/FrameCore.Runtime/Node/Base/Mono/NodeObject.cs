using System.Collections.Generic;
using UnityEngine;

namespace FrameCore.Runtime
{
   public enum NodeType
   {
      MapStage,
      MapNode,
      UIPanel,
      UINode,
   }

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

      public NodeKey Key => node.Key;

      public BaseNode node { get; private set; }
      public NodeObject Parent { get; private set; }
      public List<NodeObject> Childes { get; private set; }

      private HashSet<int> _childHashSet;

      private bool _isInit;

      public T GetGo<T>(string key) where T : Object => m_objectCollector[key] as T;

      public void SetActive(bool isActive) => gameObject.SetActive(isActive);

      public void Load()
      {
         if (_isInit)
            return;
      
         _childHashSet = new HashSet<int>();
         Childes = new List<NodeObject>();
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

         // 静态绑定的node
         var staticNodes = gameObject.GetComponent<StaticNodeCollector>().NodeObjects;
         foreach (var nodeObject in staticNodes)
         {
            nodeObject.Parent = this;
            AddChild(nodeObject);
         }

         if (gameObject.transform.parent != null)
            Parent = gameObject.transform.parent.GetComponent<NodeObject>();

         var nodeFactory = key.nodeBuilderDelegate();
         node = nodeFactory.Build(key);
         nodeFactory.NodeInit(node, this);
         _isInit = true;
      }

      public void Open(params object[] args)
      {
         if (!gameObject.activeSelf)
         {
            gameObject.SetActive(true);
         }

         Load();
         node?.Refresh(args);
      }

      public void AddChild(NodeObject nodeObject)
      {
         if (_childHashSet.Contains(nodeObject.Id))
         {
            FrameDebugger.LogError($"AddChild node:{nodeObject.name} is already exist !!!");
            return;
         }

         Childes.Add(nodeObject);
         _childHashSet.Add(nodeObject.Id);
      }

      public void RemoveChild(NodeObject nodeObject)
      {
         if (!_childHashSet.Contains(nodeObject.Id))
         {
            FrameDebugger.LogError($"RemoveChild node :{nodeObject.name} is no exist !!!");
            return;
         }

         Childes.Remove(nodeObject);
         _childHashSet.Remove(nodeObject.Id);
      }

      // #region Controller
      //
      // public T GetController<T>() where T : IController => node.GetController<T>();
      // public T GetController<T>(Func<T, bool> func) where T : IController => node.GetController(func);
      // public List<IController> GetAllController() => node.GetAllController();
      // public List<T> GetControllers<T>(Func<T, bool> func) where T : IController => node.GetControllers(func);
      //
      // #endregion

      #region 生命周期

      private void Awake()
      {
         Load();
         node?.Init();
      }

      private void OnEnable()
      {
         node?.Show();
      }

      private void Update()
      {
         node?.Update();
      }

      private void LateUpdate()
      {
         node?.LateUpdate();
      }

      private void OnDisable()
      {
         node?.Hide();
      }

      private void OnDestroy()
      {
         node?.Stop();
         Parent = null;
      }

      #endregion
   }
}