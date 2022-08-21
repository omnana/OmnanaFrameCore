namespace FrameCore.Runtime
{
    public abstract class UINodeBaseVO : BaseNodeVO
    {
        public override void OpenNode(NodeKey key, params object[] args)
        {
            IocContainer.Resolve<IUIModule>().OpenNode((UINodeObject) NodeObj, key, args);
        }

        public override void Close(bool destroy = false)
        {
            switch (NodeObj.NodeType)
            {
                case NodeType.UIPanel:
                    if (!destroy)
                        IocContainer.Resolve<IUIModule>().Close((UIPanelKey) NodeObj.Key);
                    else
                        IocContainer.Resolve<IUIModule>().Destroy((UIPanelKey) NodeObj.Key);

                    break;
                case NodeType.UINode:
                    if (!destroy)
                        IocContainer.Resolve<IUIModule>().CloseNode((UINodeObject) NodeObj);
                    else
                        RemoveNode(NodeObj);

                    break;
            }
        }

        public override void CloseNode(NodeObject child)
        {
            IocContainer.Resolve<IUIModule>().CloseNode((UINodeObject) child);
        }

        public override void RemoveNode(NodeObject child)
        {
            IocContainer.Resolve<IUIModule>().RemoveNode((UINodeObject) child);
        }
    }
}
