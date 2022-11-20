namespace FrameCore.Runtime
{
    public interface IEntity
    {
        int Id { get; set; }
        BaseEntityContext BaseContext { get; set; }
        EntityObject Object { get; set; }
        void SetActive(bool isActive);
        void Gen();
        void Init();
        void Enable();
        void SetData(BaseEntityData data);
        void Disable();
        void Destroy();
    }
}
