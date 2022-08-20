using UnityEngine;
using UnityEngine.Events;

namespace FrameCore.Runtime
{
    public class OnItemDrawEvent : UnityEvent<int, GameObject> { }
    public class OnItemClickEvent : UnityEvent<int, GameObject> { }
    public interface IUIList
    {
        int Count { get; set; }
        OnItemDrawEvent OnStart { get; set; }
        OnItemDrawEvent OnStop { get; set; }
        OnItemClickEvent OnItemClickEvent { get; set; }
        GameObject GetItem(int index);
    }
}