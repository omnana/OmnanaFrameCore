namespace FrameCore.Runtime
{
    public abstract class UINodeBaseVO : BaseNodeVO
    {
        private IUIModule UIModule => IocContainer.Resolve<IUIModule>();

        public override NodeObject OpenNode(NodeKey key, params object[] args)
        {
            return UIModule.OpenNode((UINodeObject) NodeObj, key, args);
        }

        public override void Close(bool destroy = false)
        {
            switch (NodeObj.NodeType)
            {
                case NodeType.UIPanel:
                    if (!destroy)
                        UIModule.Close((UIPanelKey) NodeObj.Key);
                    else
                        UIModule.Destroy((UIPanelKey) NodeObj.Key);

                    break;
                case NodeType.UINode:
                    if (!destroy)
                        CloseNode(NodeObj);
                    else
                        RemoveNode(NodeObj);

                    break;
            }
        }

        public override void CloseNode(NodeObject child)
        {
            UIModule.CloseNode((UINodeObject) child);
        }

        public override void RemoveNode(NodeObject child)
        {
            UIModule.RemoveNode((UINodeObject) child);
        }
    }
}
