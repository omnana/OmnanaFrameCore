namespace FrameCore.Runtime
{
    public class MapNodeKey : NodeKey
    {
        public MapNodeKey(string key, string keyName, NodeBuilderCreateHandle nodeBuilderDelegate) : base(key, keyName,
            nodeBuilderDelegate)
        {
        }
    }
}