using System;

namespace FrameCore.Runtime
{
    public interface IUISpine
    {
        /// <summary>
        /// 设置spine
        /// </summary>
        /// <param name="assetName"></param>
        void SetSpine(string assetName);
        /// <summary>
        /// 异步设置spine
        /// </summary>
        /// <param name="assetName"></param>
        void SetSpineAsync(string assetName, Action callback);
        /// <summary>
        /// 设置皮肤
        /// </summary>
        /// <param name="skinName"></param>
        void SetSkin(string skinName);
        /// <summary>
        /// 设置部件
        /// </summary>
        /// <param name="skinName"></param>
        void SetAttachment(string slotName, string attachmentName);
        /// <summary>
        /// 展示某一时刻，ratio(0 ~ 1)
        /// </summary>
        /// <param name="ratio"></param>
        void SetFrame(string animationName, float ratio);
        /// <summary>
        /// 播放动画
        /// </summary>
        /// <param name="animationName"></param>
        void Play(string animationName, bool loop);
        /// <summary>
        /// 
        /// </summary>
        void Pause();
        /// <summary>
        /// 
        /// </summary>
        void Resume();
        /// <summary>
        /// 
        /// </summary>
        void Stop();

        void Dispose();
    }
}