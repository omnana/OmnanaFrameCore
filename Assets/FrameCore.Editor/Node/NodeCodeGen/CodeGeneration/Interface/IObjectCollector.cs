using System.Collections.Generic;
using UnityEngine;

namespace FrameCore.Editor
{
    public interface IObjectCollector
    {
        Object this[string key] { get; }
        List<string> Keys { get; }
        List<Object> Objects { get; }
    }
}