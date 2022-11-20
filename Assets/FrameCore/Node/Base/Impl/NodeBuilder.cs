namespace FrameCore.Runtime
{
    public abstract class NodeBuilder
    {
        public BaseNode Build(NodeKey key)
        {
            BaseNode node = null;
            switch (key)
            {
                case MapNodeKey _:
                case MapStageKey _:
                    node = new MapNode {Key = key};
                    break;
                case UIPanelKey _:
                case UINodeKey _:
                    node = new UINode() {Key = key};
                    break;
            }

            return node;
        }

        public void NodeInit(BaseNode node, NodeObject obj)
        {
            InitStaticNodeKey(node);
            AddController(node, obj);
        }

        protected abstract void InitStaticNodeKey(BaseNode node);
        protected abstract void AddController(BaseNode node, NodeObject obj);
    }
}
