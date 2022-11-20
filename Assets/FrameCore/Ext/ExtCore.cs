using System;
using System.Collections.Generic;
using UnityEngine;

namespace FrameCore.Runtime
{
    public delegate void OnExtCoreUpdateDelegate();

    public static class ExtCore
    {
        private static MonoBehaviour _mono;
        private static readonly HashSet<Type> UpdateHs = new HashSet<Type>();
        private static readonly HashSet<Type> LateUpdateHs = new HashSet<Type>();
        private static readonly HashSet<Type> FixedUpdateHs = new HashSet<Type>();
        private static OnExtCoreUpdateDelegate _extCoreUpdateDelegate;
        private static OnExtCoreUpdateDelegate _extCoreLateUpdateDelegate;
        private static OnExtCoreUpdateDelegate _extCoreFixedUpdateDelegate;

        public static void Init(MonoBehaviour mono)
        {
            _mono = mono;
            IocContainer.ContainerResolveHandle = container =>
            {
                CheckUpdater(container);
                CheckMono(container);
            };
            IocContainer.ContainerRemoveHandle = RemoveUpdater;
        }

        private static void CheckUpdater(object target)
        {
            if (target is IUpdater updater && !UpdateHs.Contains(updater.GetType()))
            {
                _extCoreUpdateDelegate += updater.Update;
                UpdateHs.Add(updater.GetType());
            }

            if (target is ILateUpdater lateUpdater && !LateUpdateHs.Contains(lateUpdater.GetType()))
            {
                _extCoreLateUpdateDelegate += lateUpdater.LateUpdate;
                LateUpdateHs.Add(lateUpdater.GetType());
            }

            if (target is IFixedUpdater fixedUpdater && !FixedUpdateHs.Contains(fixedUpdater.GetType()))
            {
                _extCoreFixedUpdateDelegate += fixedUpdater.FixedUpdate;
                FixedUpdateHs.Add(fixedUpdater.GetType());
            }
        }
        
        private static void RemoveUpdater(object target)
        {
            if (target is IUpdater updater && UpdateHs.Contains(updater.GetType()))
            {
                _extCoreUpdateDelegate -= updater.Update;
                UpdateHs.Add(updater.GetType());
            }

            if (target is ILateUpdater lateUpdater && LateUpdateHs.Contains(lateUpdater.GetType()))
            {
                _extCoreLateUpdateDelegate -= lateUpdater.LateUpdate;
                LateUpdateHs.Add(lateUpdater.GetType());
            }

            if (target is IFixedUpdater fixedUpdater && FixedUpdateHs.Contains(fixedUpdater.GetType()))
            {
                _extCoreFixedUpdateDelegate -= fixedUpdater.FixedUpdate;
                FixedUpdateHs.Add(fixedUpdater.GetType());
            }
        }

        internal static void CheckMono(object target)
        {
            if (!(target is IMonoGetter monoGetter))
                return;

            monoGetter.GetMono(_mono);
        }

        public static void Update()
        {
            _extCoreUpdateDelegate?.Invoke();
        }

        public static void LateUpdate()
        {
            _extCoreLateUpdateDelegate?.Invoke();
        }

        public static void FixedUpdate()
        {
            _extCoreFixedUpdateDelegate?.Invoke();
        }
    }
}