using FrameCore.Runtime;
using UnityEngine;

namespace GameLogic
{
    public class UIGameStartPanelController : UINodeBaseController<UIGameStartPanelVO>
    {
        private OTimer _oTimer;

        protected override void OnInit()
        {
            VO.StartBtn.OnPressDown = OnStartClick;
            _oTimer = OTimer.Register(1, () => { Debug.Log("123"); }, isLooped: true);
        }

        protected override void OnStop()
        {
            _oTimer?.Cancel();
            _oTimer = null;
        }

        private void OnStartClick()
        {
            Debug.Log("登入");
        }
    }
}
