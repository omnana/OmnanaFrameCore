using System;
using UnityEngine;

namespace FrameCore.Editor
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class CodeGenAttribute : Attribute
    {
        public Type type { get; }

        public CodeGenAttribute(Type type)
        {
            if (typeof(MonoBehaviour).IsAssignableFrom(type))
            {
                this.type = type;
            }
            else throw new Exception($"CodeGenAttribute: the MonoBehaviour isn't implemented by the Type: {type.Name}");
        }
    }
}