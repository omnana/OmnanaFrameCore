using System;
using System.Collections.Generic;

namespace FrameCore.Runtime
{
    // public abstract class BaseNodeVO : IParentVOGetter, IChildVOGetter, IControllerGetter
    public abstract class BaseNodeVO : IParentVOGetter, IChildVOGetter
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

        #region VO

        public virtual void SetObj(NodeObject obj) => NodeObj = obj;

        public T GetParentVO<T>() where T : BaseNodeVO => NodeObj.Parent == null ? default : (T) NodeObj.Parent.node.VO;

        public List<BaseNodeVO> GetAllChildVO()
        {
            var result = new List<BaseNodeVO>();
            foreach (var nodeObject in NodeObj.Childes)
            {
                result.Add(nodeObject.node.VO);
            }

            return result;
        }

        public List<T> GetChildVOs<T>(Func<T, bool> func) where T : BaseNodeVO
        {
            var result = new List<T>();
            foreach (var nodeObject in NodeObj.Childes)
            {
                var vo = (T) nodeObject.node.VO;
                if (func.Invoke(vo))
                {
                    result.Add(vo);
                }
            }

            return result;
        }

        public T FirstChildVO<T>() where T : BaseNodeVO
        {
            var type = typeof(T);
            foreach (var nodeObject in NodeObj.Childes)
            {
                if (nodeObject.node?.VO?.GetType() == type)
                {
                    return (T)nodeObject.node.VO;
                }
            }

            return default;
        }

        public T GetChildVO<T>(Func<T, bool> func) where T : BaseNodeVO
        {
            foreach (var nodeObject in NodeObj.Childes)
            {
                var vo = (T) nodeObject.node.VO;
                if (func.Invoke(vo))
                {
                    return vo;
                }
            }

            return default;
        }

        #endregion

        #region Node相关

        public virtual int OpenNode(NodeKey key, params object[] args)
        {
            return 0;
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
