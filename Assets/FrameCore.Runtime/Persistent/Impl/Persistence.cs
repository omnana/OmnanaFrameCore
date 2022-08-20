namespace FrameCore.Runtime
{
    public static class Persistence
    {
        static IPersistence _persistence = new PlayerPrefsPersistence();

        /// <summary>
        /// Key
        /// </summary>
        public const string Prefix = "ideal";

        public static bool Write<T>(PersistenceKey key, T val)
        {
            return _persistence.Write(Prefix + key, val);
        }

        public static T Read<T>(PersistenceKey key, T defaultValue)
        {
            return _persistence.Read(Prefix + key, defaultValue);
        }

        public static void Delete(PersistenceKey key)
        {
            _persistence.Delete(Prefix + key);
        }

        public static void Clear()
        {
            _persistence.Clear();
        }
    }
}
