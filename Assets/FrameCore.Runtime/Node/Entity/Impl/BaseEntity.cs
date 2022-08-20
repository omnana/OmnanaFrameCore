namespace FrameCore.Runtime
{
    public abstract class BaseEntity<T, TM> : IEntity where T : BaseEntityContext where TM : BaseEntityData
    {
        public int Id { get; set; }
        public BaseEntityContext BaseContext { get; set; }
        public EntityObject Object { get; set; }

        public T Context { get; private set; }
        public TM Data { get; private set; }

        public virtual void Gen()
        {
        }

        public void Init()
        {
            Context = (T) BaseContext;
            OnInit();
        }
        public void Enable() => OnEnable();
        
        public void SetData(BaseEntityData data)
        {
            Data = (TM) data;
            RefreshData();
        }

        public void Disable() => OnDisable();
        public void Destroy() => OnDestroy();

        public void SetActive(bool isActive)
        {
            Object.gameObject.SetActive(isActive);
            if (Object.gameObject.activeSelf)
            {
                Enable();
            }
            else
            {
                Disable();
            }
        }

        protected virtual void OnInit()
        {
        }
            
        protected virtual void OnEnable()
        {
        }

        protected virtual void RefreshData()
        {
        }

        protected virtual void OnDisable()
        {
        }

        protected virtual void OnDestroy()
        {
        }
    }
}
