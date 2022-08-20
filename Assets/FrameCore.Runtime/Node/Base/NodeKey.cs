namespace FrameCore.Runtime
{
    public delegate NodeBuilder NodeBuilderCreateHandle();

    public class NodeKey
    {
        protected string _key;

        protected string _keyName;
        public NodeBuilderCreateHandle nodeBuilderDelegate { get; set; }

        public NodeKey(string key, string keyName, NodeBuilderCreateHandle nodeBuilderDelegate)
        {
            _key = key;
            _keyName = keyName;
            this.nodeBuilderDelegate = nodeBuilderDelegate;
            NodeKeyCollector.AddKey(_keyName, this);
        }
        
        public override string ToString()
        {
            return _key;
        }
    }
}