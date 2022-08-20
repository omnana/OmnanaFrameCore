using System;
using System.Collections.Generic;

namespace FrameCore.Runtime
{
    public interface IControllerGetter
    {
        T GetController<T>() where T : IController;
        T GetController<T>(Func<T, bool> func) where T : IController;
        List<IController> GetAllController();
        List<T> GetControllers<T>(Func<T, bool> func) where T : IController;
    }
}
