namespace FrameCore.Runtime
{
    public abstract class MapNodeBaseVO : BaseNodeVO
    {
        public override int OpenNode(NodeKey key, params object[] args)
        {
            return IocContainer.Resolve<IMapModule>().OpenNode((MapNodeObject) NodeObj, key, args);
        }

        public override void Close(bool destroy = false)
        {
            switch (NodeObj.NodeType)
            {
                case NodeType.MapStage:
                    if (!destroy)
                        IocContainer.Resolve<IMapModule>().CloseStage((MapStageKey) NodeObj.Key);
                    else
                        IocContainer.Resolve<IMapModule>().RemoveNode((MapNodeObject) NodeObj);
                    break;
                case NodeType.MapNode:
                    if (!destroy)
                        IocContainer.Resolve<IMapModule>().CloseNode((MapNodeObject) NodeObj);
                    else
                        IocContainer.Resolve<IMapModule>().RemoveNode((MapNodeObject) NodeObj);
                    break;
            }
        }

        public override void CloseNode(NodeObject child)
        {
            IocContainer.Resolve<IMapModule>().CloseNode((MapNodeObject) child);
        }

        public override void RemoveNode(NodeObject child)
        {
            IocContainer.Resolve<IMapModule>().RemoveNode((MapNodeObject) child);
        }
    }
}
