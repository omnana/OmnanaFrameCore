using FrameCore.Runtime;

namespace GameLogic
{
    public static partial class UIPanelKeys
    {
        public static readonly UIPanelKey UIGameStartPanel = new UIPanelKey("UIGameStartPanel","UIGameStartPanel", () => new UIGameStartPanelBuilder());
    }

    public class UIGameStartPanelBuilder : NodeBuilder
    {
        // add static node's key
        protected override void InitStaticNodeKey(BaseNode node)
        {
        }

        // add static node's controllers
        protected override void AddController(BaseNode node, NodeObject obj)
        {
            node.AddController<UIGameStartPanelController, UIGameStartPanelVO>(obj);
        }
    }
}
