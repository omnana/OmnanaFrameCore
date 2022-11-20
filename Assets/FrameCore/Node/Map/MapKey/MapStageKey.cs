namespace FrameCore.Runtime
{
    public class MapStageKey : NodeKey
    {
        public MapStageKey(string key, string keyName, NodeBuilderCreateHandle nodeBuilderDelegate) : base(key, keyName,
            nodeBuilderDelegate)
        {
        }
    }
}