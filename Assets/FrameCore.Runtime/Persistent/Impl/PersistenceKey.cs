namespace FrameCore.Runtime
{
    public class PersistenceKey
    {
        public string Key { get; }
        public PersistenceKey(string key)
        {
            Key = key;
        }

        public static implicit operator string(PersistenceKey persistenceKey)
        {
            return persistenceKey.Key;
        }
    }
}