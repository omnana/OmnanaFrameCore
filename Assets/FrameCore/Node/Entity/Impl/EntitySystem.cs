using System;
using System.Collections.Generic;
using UnityEngine;

namespace FrameCore.Runtime
{
    public class EntitySystem<T> : IEntitySystem<T>, IUpdater, IFixedUpdater where T: IEntity
    {
        private readonly Dictionary<int, IEntity> _entityDic = new Dictionary<int, IEntity>();

        private readonly List<IUpdater> _updateList = new List<IUpdater>();
        private readonly List<IUpdater> _tempUpdaterList = new List<IUpdater>();
        private readonly List<IFixedUpdater> _fixedUpdateList = new List<IFixedUpdater>();
        private readonly List<IFixedUpdater> _tempFixedUpdateList = new List<IFixedUpdater>();

        private readonly GoPool goPool = new GoPool();

        private BaseEntityContext _context;

        public int Count => _entityDic.Count;

        public T CreateEntity(EntityKey key, Transform parent = null)
        {
            GameObject go;
            if (goPool.Count == 0)
            {
                go = IocContainer.Resolve<IPrefabModule>().LoadSync($"Entity/{key}.prefab", parent);
            }
            else
            {
                go = goPool.Get();
                go.transform.SetParent(parent, false);
            }

            var entity = (T) key.EntityCreateDelegate();
            if (_context == null)
            {
                _context = key.ContextCreateDelegate();
                InjectHelper.Reflect(_context);
            }

            entity.Id = go.GetInstanceID();
            entity.BaseContext = _context;
            entity.Object = go.GetComponent<EntityObject>();
            entity.Gen();
            entity.Init();
            entity.SetActive(true);
            if (entity is IUpdater updater)
            {
                _updateList.Add(updater);
            }
            if (entity is IFixedUpdater fixedUpdater)
            {
                _fixedUpdateList.Add(fixedUpdater);
            }
            _entityDic.Add(entity.Id, entity);
            return entity;
        }

        public T CacheEntity(EntityKey key, Transform parent = null)
        {
            GameObject go = IocContainer.Resolve<IPrefabModule>().LoadSync($"Entity/{key}.prefab", parent);
            go.transform.SetParent(parent, false);
            var entity = (T) key.EntityCreateDelegate();
            if (_context == null)
            {
                _context = key.ContextCreateDelegate();
                InjectHelper.Reflect(_context);
            }

            entity.Id = go.GetInstanceID();
            entity.BaseContext = _context;
            entity.Object = go.GetComponent<EntityObject>();
            entity.Gen();
            entity.Init();
            entity.SetActive(false);
            goPool.Recycle(entity.Object.gameObject);
            return entity;
        }

        public T GetEntity(int entityId)
        {
            _entityDic.TryGetValue(entityId, out var entity);
            return (T) entity;
        }

        public void Recycle(int entityId)
        {
            if (_entityDic.ContainsKey(entityId))
            {
                var entity = _entityDic[entityId];
                if (entity is IUpdater updater)
                {
                    _updateList.Remove(updater);
                }
                if (entity is IFixedUpdater fixedUpdater)
                {
                    _fixedUpdateList.Remove(fixedUpdater);
                }
                entity.SetActive(false);
                _entityDic.Remove(entityId);
                goPool.Recycle(entity.Object.gameObject);
            }
        }

        public void Destroy(int entityId)
        {
            if (_entityDic.ContainsKey(entityId))
            {
                var entity = _entityDic[entityId];
                if (entity is IUpdater updater)
                {
                    _updateList.Remove(updater);
                }
                if (entity is IFixedUpdater fixedUpdater)
                {
                    _fixedUpdateList.Remove(fixedUpdater);
                }
                entity.SetActive(false);
                entity.Destroy();
                _entityDic.Remove(entityId);
                IocContainer.Resolve<IPrefabModule>().Destroy(entity.Object.gameObject);
            }
        }

        public void Foreach(Action<T> action)
        {
            foreach (var entity in _entityDic.Values)
            {
                var entityDicValue = (T) entity;
                action?.Invoke(entityDicValue);
            }
        }

        public void RecycleAll()
        {
            foreach (var entity in _entityDic.Values)
            {
                entity.SetActive(false);
                goPool.Recycle(entity.Object.gameObject);
            }
            
            _entityDic.Clear();
            _updateList.Clear();
            _fixedUpdateList.Clear();
        }

        public void Clear()
        {
            if (_entityDic.Values.Count == 0)
                return;

            var prefabModule = IocContainer.Resolve<IPrefabModule>();
            foreach (var entity in _entityDic.Values)
            {
                if (entity.Object == null)
                {
                    continue;
                }

                prefabModule.Destroy(entity.Object.gameObject);
            }

            while (goPool.Count > 0)
            {
                var item = goPool.Get();
                prefabModule.Destroy(item);
            }
            
            _entityDic.Clear();
            _context.Clear();
            _updateList.Clear();
            _fixedUpdateList.Clear();
        }

        public bool Contains(int entityId)
        {
            return _entityDic.ContainsKey(entityId);
        }

        public void Update()
        {
            _tempUpdaterList.Clear();
            _tempUpdaterList.AddRange(_updateList);
            foreach (var updater in _tempUpdaterList)
            {
                updater.Update();
            }
        }

        public void FixedUpdate()
        {
            _tempFixedUpdateList.Clear();
            _tempFixedUpdateList.AddRange(_fixedUpdateList);
            foreach (var updater in _tempFixedUpdateList)
            {
                updater.FixedUpdate();
            }
        }
    }
}
