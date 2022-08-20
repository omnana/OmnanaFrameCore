using System.Collections.Generic;
using UnityEngine;

namespace FrameCore.Runtime
{
    public delegate void OnExtCoreUpdateDelegate();

    public static class ExtCore
    {
        private static MonoBehaviour _mono;
        private static readonly HashSet<int> UpdateHs = new HashSet<int>();
        private static readonly HashSet<int> LateUpdateHs = new HashSet<int>();
        private static readonly HashSet<int> FixedUpdateHs = new HashSet<int>();
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

            // var types = IocContainer.GetTypeList();
            // foreach (var type in types)
            // {
            //     CheckUpdater(IocContainer.Resolve(type));
            //     CheckMono(IocContainer.Resolve(type));
            // }
        }

        // 如果需要对外开放，再改成public
        internal static void CheckUpdater(object target)
        {
            if (target is IUpdater updater && !UpdateHs.Contains(updater.GetHashCode()))
            {
                _extCoreUpdateDelegate += updater.Update;
                UpdateHs.Add(updater.GetHashCode());
            }

            if (target is ILateUpdater lateUpdater && !LateUpdateHs.Contains(lateUpdater.GetHashCode()))
            {
                _extCoreUpdateDelegate += lateUpdater.LateUpdate;
                LateUpdateHs.Add(lateUpdater.GetHashCode());
            }

            if (target is IFixedUpdater fixedUpdater && !FixedUpdateHs.Contains(fixedUpdater.GetHashCode()))
            {
                _extCoreUpdateDelegate += fixedUpdater.FixedUpdate;
                FixedUpdateHs.Add(fixedUpdater.GetHashCode());
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