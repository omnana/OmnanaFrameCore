using FrameCore.Runtime;

namespace GameLogic
{
    public partial class UIGameStartPanelVO : UINodeBaseVO
    {
        public UIPressButton StartBtn => _startBtn;

        protected override void OnInit()
        {
            _uIGameStartSubNode.GetVO<UIGameStartSubNodeVO>().SetTitle("我是你爹");
        }
    }
}
