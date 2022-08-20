using System;
using System.Collections.Generic;

namespace FrameCore.Runtime
{
    public interface IChildVOGetter
    {
        List<BaseNodeVO> GetAllChildVO();
        List<T> GetChildVOs<T>(Func<T, bool> func) where T : BaseNodeVO;
        T FirstChildVO<T>() where T : BaseNodeVO;
        T GetChildVO<T>(Func<T, bool> func) where T : BaseNodeVO;
    }
}
