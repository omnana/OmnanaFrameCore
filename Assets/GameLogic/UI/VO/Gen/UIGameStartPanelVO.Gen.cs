using FrameCore.Runtime;

namespace GameLogic
{
    // 禁止手动修改
    public partial class UIGameStartPanelVO
    {
        private UIPressButton _startBtn;

        public override void SetObj(NodeObject obj)
        {
            base.SetObj(obj);
            _startBtn = obj.GetGo<UIPressButton>("StartBtn");
        }
    }
}
