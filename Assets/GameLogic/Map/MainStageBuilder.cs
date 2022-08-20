using FrameCore.Runtime;

namespace GameLogic
{
    public static partial class MapStageKeys
    {
        public static readonly MapStageKey MainStage = new MapStageKey("MainStage","MainStage", () => new MainStageBuilder());
    }

    public class MainStageBuilder : NodeBuilder
    {
        // add static node's key
        protected override void InitStaticNodeKey(BaseNode node)
        {
        }

        // add static node's controllers
        protected override void AddController(BaseNode node, NodeObject obj)
        {
            node.AddController<MainStageController, MainStageVO>(obj);
        }
    }
}
