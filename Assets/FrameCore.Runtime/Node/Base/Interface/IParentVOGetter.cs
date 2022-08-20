namespace FrameCore.Runtime
{
    public interface IParentVOGetter
    {
        T GetParentVO<T>() where T : BaseNodeVO;
    }
}
