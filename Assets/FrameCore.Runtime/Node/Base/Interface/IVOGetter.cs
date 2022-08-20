namespace FrameCore.Runtime
{
    public interface IVOGetter
    {
        T GetVO<T>() where T : BaseNodeVO;
    }
}
