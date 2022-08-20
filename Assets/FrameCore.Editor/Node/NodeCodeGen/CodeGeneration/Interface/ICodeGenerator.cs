using FrameCore.Runtime;

namespace FrameCore.Editor
{
    public interface ICodeGenerator : IGenerator
    {
        ObjectCollector[] ObjectCollectors { get; }
    }
}
