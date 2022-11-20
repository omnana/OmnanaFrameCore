using FrameCore.Runtime;

namespace FrameCore.Editor
{
    public interface ICodeGenerator
    {
        bool Legal { get; }
        ObjectCollector[] ObjectCollectors { get; }
        void Generate();
    }
}
