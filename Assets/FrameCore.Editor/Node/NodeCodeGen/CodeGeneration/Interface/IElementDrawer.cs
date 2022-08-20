using UnityEngine;

namespace FrameCore.Editor
{
    public interface IElementDrawer
    {
        void Draw(GameObject go, string product, string name);
    }

}
