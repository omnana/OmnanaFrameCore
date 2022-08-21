namespace FrameCore.Runtime
{
    public interface IUIModule
    {
        void Open(UIPanelKey key);
        void Close(UIPanelKey key);
        void Destroy(UIPanelKey key);
        UINodeObject OpenNode(UINodeObject parent, NodeKey key, params object[] args);
        void CloseNode(UINodeObject node);
        void RemoveNode(UINodeObject child);
    }
}
