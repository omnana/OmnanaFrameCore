using System;

namespace FrameCore.Runtime
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false,Inherited = true)]
    public class InjectAttribute : Attribute
    {
    }
}
