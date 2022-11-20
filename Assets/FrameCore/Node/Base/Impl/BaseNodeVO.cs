namespace FrameCore.Runtime
{
    public abstract class BaseNodeVO
    {
        public int Id => NodeObj.Id;

        protected NodeObject NodeObj;

        #region 生命周期

        public void SetActive(bool active) => NodeObj.SetActive(active);

        public void Init() => OnInit();

        protected virtual void OnInit()
        {
        }

        public void Destroy() => OnDestroy();

        protected virtual void OnDestroy()
        {
        }
        
        #endregion
    
        public virtual void SetObj(NodeObject obj) => NodeObj = obj;

        #region Node相关

        public virtual NodeObject OpenNode(NodeKey key, params object[] args)
        {
            return NodeObj;
        }

        public virtual void Close(bool destroy = false)
        {
        }

        public virtual void CloseNode(NodeObject child)
        {
        }

        public virtual void RemoveNode(NodeObject child)
        {
        }

        #endregion
    }
}
