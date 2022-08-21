using FrameCore.Runtime;

namespace GameLogic
{
    public static partial class UINodeKeys
    {
        public static readonly UINodeKey UIGameStartSubNode = new UINodeKey("Node/UIGameStartSubNode","UIGameStartSubNode", () => new UIGameStartSubNodeBuilder());
    }

    public class UIGameStartSubNodeBuilder : NodeBuilder
    {
        // add static node's key
        protected override void InitStaticNodeKey(BaseNode node)
        {
        }

        // add static node's controllers
        protected override void AddController(BaseNode node, NodeObject obj)
        {
            node.AddController<UIGameStartSubNodeController, UIGameStartSubNodeVO>(obj);
        }
    }
}
