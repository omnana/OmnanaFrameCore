using System.Reflection;
using UnityEngine;

namespace FrameCore.Runtime
{
    public static class InjectHelper
    {
        private const BindingFlags Flag = BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public;
        
        // 检查injectTarget有没有可以注入的对象
        public static void Reflect(object target)
        {
            var type = target.GetType();
            var members = type.FindMembers(MemberTypes.Field, Flag, null, null);
            for (var i = 0; i < members.Length; i++)
            {
                var member = members[i];
                var injections = member.GetCustomAttributes(typeof(InjectAttribute), true);
                if (injections.Length == 0 || !(member is FieldInfo fieldInfo))
                    continue;

                var fieldType = fieldInfo.FieldType;
                var obj = IocContainer.Resolve(fieldType);
                if (obj == null)
                {
                    FrameDebugger.LogError($"type : {fieldType} 未注入！！");
                    continue;
                }

                fieldInfo.SetValue(target, obj);
            }
        }
    }
}
