namespace FrameCore.Runtime
{
    public delegate IEntity EntityCreateDelegate();

    public delegate BaseEntityContext ContextCreateDelegate();

    public class EntityKey
    {
        private string _key;

        public EntityCreateDelegate EntityCreateDelegate { get; private set; }
        public ContextCreateDelegate ContextCreateDelegate { get; private set; }

        public EntityKey(string key, EntityCreateDelegate entityCreateDelegate,
            ContextCreateDelegate contextCreateDelegate)
        {
            _key = key;
            EntityCreateDelegate = entityCreateDelegate;
            ContextCreateDelegate = contextCreateDelegate;
        }

        public override string ToString()
        {
            return _key;
        }
    }
}
