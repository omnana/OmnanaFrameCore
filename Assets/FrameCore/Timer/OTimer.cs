using System;
using UnityEngine;

namespace FrameCore.Runtime
{
    public class OTimer
    {
        #region 公共属性或字段

        /// <summary>
        /// How long the timer takes to complete from start to finish.
        /// </summary>
        public float Duration { get; private set; }

        /// <summary>
        /// Whether the timer will run again after completion.
        /// </summary>
        public bool IsLooped { get; set; }

        /// <summary>
        /// Whether or not the timer completed running. This is false if the timer was cancelled.
        /// </summary>
        public bool IsCompleted { get; private set; }

        /// <summary>
        /// Whether the timer uses real-time or game-time. Real time is unaffected by changes to the timescale
        /// of the game(e.g. pausing, slow-mo), while game time is affected.
        /// </summary>
        public bool UsesRealTime { get; private set; }

        /// <summary>
        /// Whether the timer is currently paused.
        /// </summary>
        public bool IsPaused => this._timeElapsedBeforePause.HasValue;

        /// <summary>
        /// Whether or not the timer was cancelled.
        /// </summary>
        public bool IsCancelled => this._timeElapsedBeforeCancel.HasValue;

        /// <summary>
        /// Get whether or not the timer has finished running for any reason.
        /// </summary>
        public bool IsDone => this.IsCompleted || this.IsCancelled || this.IsOwnerDestroyed;

        #endregion
        
        #region 公共方法

        /// <summary>
        /// Stop a timer that is in-progress or paused. The timer's on completion callback will not be called.
        /// </summary>
        public void Cancel()
        {
            if (this.IsDone)
            {
                return;
            }

            this._timeElapsedBeforeCancel = this.GetTimeElapsed();
            this._timeElapsedBeforePause = null;
        }

        /// <summary>
        /// Pause a running timer. A paused timer can be resumed from the same point it was paused.
        /// </summary>
        public void Pause()
        {
            if (this.IsPaused || this.IsDone)
            {
                return;
            }

            this._timeElapsedBeforePause = this.GetTimeElapsed();
        }

        /// <summary>
        /// Continue a paused timer. Does nothing if the timer has not been paused.
        /// </summary>
        public void Resume()
        {
            if (!this.IsPaused || this.IsDone)
            {
                return;
            }

            this._timeElapsedBeforePause = null;
        }

        /// <summary>
        /// Get how many seconds have elapsed since the start of this timer's current cycle.
        /// </summary>
        /// <returns>The number of seconds that have elapsed since the start of this timer's current cycle, i.e.
        /// the current loop if the timer is looped, or the start if it isn't.
        ///
        /// If the timer has finished running, this is equal to the duration.
        ///
        /// If the timer was cancelled/paused, this is equal to the number of seconds that passed between the timer
        /// starting and when it was cancelled/paused.</returns>
        public float GetTimeElapsed()
        {
            if (this.IsCompleted || this.GetWorldTime() >= this.GetFireTime())
            {
                return this.Duration;
            }

            return this._timeElapsedBeforeCancel ??
                   this._timeElapsedBeforePause ??
                   this.GetWorldTime() - this._startTime;
        }

        /// <summary>
        /// Get how many seconds remain before the timer completes.
        /// </summary>
        /// <returns>The number of seconds that remain to be elapsed until the timer is completed. A timer
        /// is only elapsing time if it is not paused, cancelled, or completed. This will be equal to zero
        /// if the timer completed.</returns>
        public float GetTimeRemaining()
        {
            return this.Duration - this.GetTimeElapsed();
        }

        /// <summary>
        /// Get how much progress the timer has made from start to finish as a ratio.
        /// </summary>
        /// <returns>A value from 0 to 1 indicating how much of the timer's duration has been elapsed.</returns>
        public float GetRatioComplete()
        {
            return this.GetTimeElapsed() / this.Duration;
        }

        /// <summary>
        /// Get how much progress the timer has left to make as a ratio.
        /// </summary>
        /// <returns>A value from 0 to 1 indicating how much of the timer's duration remains to be elapsed.</returns>
        public float GetRatioRemaining()
        {
            return this.GetTimeRemaining() / this.Duration;
        }

        #endregion
        
        #region 私有属性或字段

        private bool IsOwnerDestroyed => this._hasAutoDestroyOwner && this._autoDestroyOwner == null;

        private readonly Action _onComplete;
        private readonly Action<float> _onUpdate;
        private float _startTime;
        private float _lastUpdateTime;

        // for pausing, we push the start time forward by the amount of time that has passed.
        // this will mess with the amount of time that elapsed when we're cancelled or paused if we just
        // check the start time versus the current world time, so we need to cache the time that was elapsed
        // before we paused/cancelled
        private float? _timeElapsedBeforeCancel;
        private float? _timeElapsedBeforePause;

        // after the auto destroy owner is destroyed, the timer will expire
        // this way you don't run into any annoying bugs with timers running and accessing objects
        // after they have been destroyed
        private readonly MonoBehaviour _autoDestroyOwner;
        private readonly bool _hasAutoDestroyOwner;

        #endregion

        #region 私有方法

        private float GetWorldTime()
        {
            return UsesRealTime ? Time.realtimeSinceStartup : Time.time;
        }

        private float GetFireTime()
        {
            return _startTime + Duration;
        }

        private float GetTimeDelta()
        {
            return GetWorldTime() - _lastUpdateTime;
        }

        public void Update()
        {
            if (IsDone)
                return;

            if (IsPaused)
            {
                _startTime += GetTimeDelta();
                _lastUpdateTime = GetWorldTime();
                return;
            }

            _lastUpdateTime = GetWorldTime();
            _onUpdate?.Invoke(GetTimeElapsed());

            if (!(GetWorldTime() >= GetFireTime())) 
                return;
            
            _onComplete?.Invoke();
            if (IsLooped)
            {
                _startTime = GetWorldTime();
            }
            else
            {
                IsCompleted = true;
            }
        }

        #endregion
        
        private OTimer(float duration, Action onComplete, Action<float> onUpdate,
            bool isLooped, bool usesRealTime, MonoBehaviour autoDestroyOwner)
        {
            Duration = duration;
            _onComplete = onComplete;
            _onUpdate = onUpdate;

            IsLooped = isLooped;
            UsesRealTime = usesRealTime;

            _autoDestroyOwner = autoDestroyOwner;
            _hasAutoDestroyOwner = autoDestroyOwner != null;

            _startTime = GetWorldTime();
            _lastUpdateTime = _startTime;
        }

        #region 静态方法

        private static OTimerModule _timerModule;

        public static OTimer Register(float duration, Action onComplete, Action<float> onUpdate = null,
            bool isLooped = false, bool useRealTime = false, MonoBehaviour autoDestroyOwner = null)
        {
            if (_timerModule == null)
            {
                GameObject go = new GameObject {name = "TimerModule"};
                _timerModule = go.AddComponent<OTimerModule>();
            }

            OTimer timer = new OTimer(duration, onComplete, onUpdate, isLooped, useRealTime, autoDestroyOwner);
            _timerModule.RegisterTimer(timer);
            return timer;
        }

        public static void Cancel(OTimer timer)
        {
            timer?.Cancel();
        }

        public static void Pause(OTimer timer)
        {
            timer?.Pause();
        }

        public static void Resume(OTimer timer)
        {
            timer?.Resume();
        }

        public static void CancelAllRegisteredTimers()
        {
            if (_timerModule != null)
            {
                _timerModule.CancelAllTimers();
            }
        }

        public static void PauseAllRegisteredTimers()
        {
            if (_timerModule != null)
            {
                _timerModule.PauseAllTimers();
            }
        }

        public static void ResumeAllRegisteredTimers()
        {
            if (_timerModule != null)
            {
                _timerModule.ResumeAllTimers();
            }

        }

        #endregion
    }
}
