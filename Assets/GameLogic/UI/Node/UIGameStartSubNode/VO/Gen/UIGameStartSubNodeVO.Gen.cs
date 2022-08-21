using FrameCore.Runtime;
using UnityEngine.UI;

namespace GameLogic
{
    // 禁止手动修改
    public partial class UIGameStartSubNodeVO
    {
        private Text _title;

        public override void SetObj(NodeObject obj)
        {
            base.SetObj(obj);
            _title = obj.GetGo<Text>("Title");
        }
    }
}
