namespace FrameCore.Runtime
{
    public abstract class MapNodeBaseVO : BaseNodeVO
    {
        private IMapModule MapModule => IocContainer.Resolve<IMapModule>();

        public override NodeObject OpenNode(NodeKey key, params object[] args)
        {
            return MapModule.OpenNode((MapNodeObject) NodeObj, key, args);
        }

        public override void Close(bool destroy = false)
        {
            switch (NodeObj.NodeType)
            {
                case NodeType.MapStage:
                    if (!destroy)
                        MapModule.CloseStage((MapStageKey) NodeObj.Key);
                    else
                        MapModule.RemoveNode((MapNodeObject) NodeObj);
                    break;
                case NodeType.MapNode:
                    if (!destroy)
                        MapModule.CloseNode((MapNodeObject) NodeObj);
                    else
                        MapModule.RemoveNode((MapNodeObject) NodeObj);
                    break;
            }
        }

        public override void CloseNode(NodeObject child)
        {
            MapModule.CloseNode((MapNodeObject) child);
        }

        public override void RemoveNode(NodeObject child)
        {
            MapModule.RemoveNode((MapNodeObject) child);
        }
    }
}
