using UnityEngine;

namespace FrameCore.Runtime
{
    public class UIRoot : MonoBehaviour
    {
        public static int BaseWidth = 1080;
        public static int BaseHeight = 1920;

        public static float CanvasHeight { get; private set; }
        public static float CanvasWidth { get; private set; }

        public static float ScreenRatio { get; private set; }
        public static float BaseScreenRatio { get; private set; }

        public RectTransform BackLayer;
        public RectTransform NormalLayer;
        public RectTransform PopLayer;
        public RectTransform TopBar;
        public RectTransform BottomBar;
        public GameObject CatchScreenGo;

        public void Init()
        {
            var rect = GetComponent<RectTransform>().rect;
            CanvasHeight = rect.height;
            CanvasWidth = rect.width;
            ScreenRatio = CanvasHeight / CanvasWidth;
            BaseScreenRatio = BaseHeight / (float) BaseWidth;
            // Screen.SetResolution(BaseWidth, BaseHeight, true);
        }

        public void CheckBarState()
        {
            // if (ScreenRatio > 2)
            // {
            //     // 需要从真机取
            //     ShowTopBar(40);
            // }
            //
            // if (ScreenRatio < 1)
            // {
            //     // Screen.orientation = ScreenOrientation.LandscapeLeft;
            // }
        }

        private void ShowTopBar(float barHeight)
        {
            BackLayer.offsetMax = new Vector2(0, -barHeight);
            NormalLayer.offsetMax = new Vector2(0, -barHeight);
            PopLayer.offsetMax = new Vector2(0, -barHeight);
            TopBar.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, CanvasWidth);
            TopBar.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, barHeight);
            TopBar.gameObject.SetActive(true);
        }
    }
}
