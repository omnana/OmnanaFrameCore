using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FrameCore.Runtime;
using UnityEngine;

namespace FrameCore.Editor
{
    public static class GenerateUtility
    {
        public static bool Generate(GameObject go, Type objectType,
            HashSet<KeyValuePair<int, Type>> generateInstanceFullNameSet = null)
        {
            if (CodeWriteHelpUtil.Assemblies.Length == 0)
            {
                UnityEditor.EditorUtility.DisplayDialog(
                    "警告",
                    "请前往CodeGenConfig的Code Gen Reference下添加要生成代码所在的程序集。 谢谢！",
                    "确认"
                );
                return false;
            }

            ICodeGenerator generator = GetCodeGenerator(go, objectType);
            if (generator == null)
                return true;

            if (!generator.Legal)
                return false;

            ObjectCollector[] objectCollectors = generator.ObjectCollectors;
            if (objectCollectors == null || objectCollectors.Length == 0) return false;
            // if (generateInstanceFullNameSet == null)
            //     generateInstanceFullNameSet = new HashSet<KeyValuePair<int, Type>>();

            // foreach (var objectCollector in objectCollectors)
            // {
            //     if (objectCollector == null) continue;
            //
            //     List<UnityEngine.Object>.Enumerator enumValues = objectCollector.Objects.GetEnumerator();
            //     while (enumValues.MoveNext())
            //     {
            //         if (enumValues.Current == null) continue;
            //
            //         GameObject next = enumValues.Current as GameObject ?? (enumValues.Current as Component).gameObject;
            //
            //         generateInstanceFullNameSet.Add(new KeyValuePair<int, Type>(go.GetInstanceID(), objectType));
            //         if (generateInstanceFullNameSet.Contains(new KeyValuePair<int, Type>(next.GetInstanceID(),
            //             objectType)))
            //         {
            //             EditorUtility.DisplayDialog("重复绑定",
            //                 $"出现绑定闭环（如自己绑定自己，A绑B，B绑A等情况", "我知道了");
            //             return false;
            //         }
            //
            //         if (!Generate(next, enumValues.Current.GetType(), generateInstanceFullNameSet))
            //         {
            //             return false;
            //         }
            //     }
            // }

            generator.Generate();
            return true;
        }

        // 获取代码生成器
        private static ICodeGenerator GetCodeGenerator(GameObject go, Type genObjType)
        {
            return CodeWriteHelpUtil.Assemblies.SelectMany(assembly => assembly.GetTypes()
            ).Where(
                type => type.GetCustomAttribute<CodeGenAttribute>() != null
            ).Where(
                type => type.GetCustomAttribute<CodeGenAttribute>().type == genObjType
            ).Select(
                type => Activator.CreateInstance(type, new object[] {go}) as ICodeGenerator
            ).FirstOrDefault();
        }
    }
}
