using FrameCore.Runtime;

namespace GameLogic
{
    public partial class UIGameStartPanelVO : UINodeBaseVO
    {
        public UIPressButton StartBtn => _startBtn;
        
        protected override void OnInit()
        {
            GetAllChildVO()
        }
    }
}
