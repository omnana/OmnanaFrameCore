using FrameCore.Runtime;

namespace GameLogic
{
    public partial class UIGameStartSubNodeVO : UINodeBaseVO
    {
        protected override void OnInit()
        {
        }

        public void SetTitle(string title)
        {
            _title.text = title;
        }
    }
}
