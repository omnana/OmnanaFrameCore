namespace FrameCore.Editor
{
    public interface IGenerator
    {
        bool Legal { get; }

        void Generate();
    }
}
