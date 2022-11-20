using System;
using UnityEngine;

namespace FrameCore.Runtime
{
    public interface IEntitySystem<T> where T : IEntity
    {
        int Count { get; }
        T CreateEntity(EntityKey key, Transform parent = null);
        T CacheEntity(EntityKey key, Transform parent = null);
        T GetEntity(int entityId);
        void Recycle(int entityId);
        void Destroy(int entityId);
        bool Contains(int entityId);
        void Foreach(Action<T> action);
        void RecycleAll();
        void Clear();
    }
}