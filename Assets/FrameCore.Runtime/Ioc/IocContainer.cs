using System;
using System.Collections.Generic;
using UnityEngine;

namespace FrameCore.Runtime
{
    public class TypeInfo
    {
        private Type _bindType;
        public Type targetType;
        public object Target;
        public bool IsReflect { get; private set; }

        public TypeInfo(Type type)
        {
            _bindType = type;
        }
        
        public TypeInfo To(Type t)
        {
            targetType = t;
            return this;
        }
        
        public object New()
        {
            Target = Activator.CreateInstance(targetType);
            return Target;
        }

        public void SetTarget(object target)
        {
            targetType = target.GetType();
            Target = target;
        }

        // 如果它也需要主动检查注入
        public void Reflect()
        {
            IsReflect = true;
        }
    }

    public delegate void ContainerResolveHandle(object container);

    public static class IocContainer
    {
        private static readonly Dictionary<Type, TypeInfo> InfoDict = new Dictionary<Type, TypeInfo>();

        public static ContainerResolveHandle ContainerResolveHandle { get; set; }

        public static TypeInfo Bind(Type type)
        {
            var typeInfo = new TypeInfo(type);
            InfoDict.Add(type, typeInfo);
            return typeInfo;
        }

        public static void RegisterSingleton(Type bindType, object instance)
        {
            if (InfoDict.ContainsKey(bindType))
            {
                return;
            }

            var info = new TypeInfo(bindType);
            info.To(instance.GetType()).SetTarget(instance);
            InfoDict.Add(bindType, info);
        }

        public static void UnBind(Type type)
        {
            if (InfoDict.ContainsKey(type))
                InfoDict.Remove(type);
        }

        public static T Resolve<T>() where T : class
        {
            var type = typeof(T);
            if (!InfoDict.ContainsKey(type))
            {
                Debug.LogError($"未绑定type : {type}");
                return default;
            }

            var target = InfoDict[type].Target ?? InfoDict[type].New();
            if (InfoDict[type].IsReflect)
            {
                InjectHelper.Reflect(target);
            }

            ContainerResolveHandle?.Invoke(target);
            return target as T;
        }

        public static object Resolve(Type type)
        {
            if (!InfoDict.ContainsKey(type))
            {
                Debug.LogError($"未绑定type : {type}");
                return null;
            }

            var target = InfoDict[type].Target ?? InfoDict[type].New();
            if (InfoDict[type].IsReflect)
            {
                InjectHelper.Reflect(target);
            }

            ContainerResolveHandle?.Invoke(target);
            return target;
        }
    }
}