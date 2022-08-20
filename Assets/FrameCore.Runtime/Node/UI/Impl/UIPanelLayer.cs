using System.ComponentModel;

namespace FrameCore.Runtime
{
    public enum UIPanelLayer
    {
        [Description("UIRoot")] UIRoot,
        [Description("Back")] Back,
        [Description("Normal")] Normal,
        [Description("Top")] Top,
        [Description("Pop")] Pop,
        [Description("Mask")] Mask,
    }
}