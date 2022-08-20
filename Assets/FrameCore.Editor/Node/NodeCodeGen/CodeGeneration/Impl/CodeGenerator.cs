using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using FrameCore.Runtime;

namespace FrameCore.Editor
{
    public abstract class CodeGenerator : ICodeGenerator
    {
        private readonly HashSet<string> nameSpaceHashSet = new HashSet<string>();
        private MonoBehaviour _target;

        public abstract bool Legal { get; }
        public ObjectCollector[] ObjectCollectors => GetObjectCollectors();
        public abstract void Generate();
        public abstract ObjectCollector[] GetObjectCollectors();

        protected readonly BindingFlags Flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
        protected readonly string Product;
        protected readonly string Name;
        protected readonly GameObject GO;
        protected ObjectCollector _objectCollector;

        protected CodeGenerator(GameObject go)
        {
            GO = go;
            Product = GameConfigHelper.GetProduct();
            Name = CodeWriteHelpUtil.GetPrefabOriginalName(go);
        }

        protected void WriteNamespace(CodeWriter writer, ObjectCollector objectCollector)
        {
            nameSpaceHashSet.Clear();
            nameSpaceHashSet.Add($"using FrameCore.Runtime;");
            CodeWriteHelpUtil.WriteNameSpace(objectCollector, nameSpaceHashSet);
            foreach (var item in nameSpaceHashSet)
            {
                writer.WriteLine(item);
            }
        }

        protected void LoadObjectCollector<T>() where T : MonoBehaviour
        {
            _target = GO.GetComponent<T>();
            var value = _target.GetType().BaseType.GetField("m_objectCollector", Flags);
            _objectCollector = value.GetValue(_target) as ObjectCollector;
        }

        protected T GetTarget<T>() where T : MonoBehaviour
        {
            return _target as T;
        }
    }
}
