using UnityEngine;

namespace FrameCore.Runtime
{
    public interface IMapModule
    {
        Camera CurrentStageCam { get; }
        void RunStage(MapStageKey key);
        void CloseStage(MapStageKey key);
        void DestroyStage(MapStageKey key);
        NodeObject OpenNode(MapNodeObject parent, NodeKey mapNodeKey, params object[] args);
        void CloseNode(MapNodeObject node);
        void RemoveNode(MapNodeObject node);
    }
}