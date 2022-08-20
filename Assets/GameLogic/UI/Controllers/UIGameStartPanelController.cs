using FrameCore.Runtime;
using UnityEngine;

namespace GameLogic
{
    public class UIGameStartPanelController : UINodeBaseController<UIGameStartPanelVO>
    {
        protected override void OnInit()
        {
            VO.StartBtn.OnPressDown = OnStartClick;
        }

        private void OnStartClick()
        {
            Debug.Log("登入");
        }
    }
}
