using System.Collections.Generic;
using UnityEngine;

namespace FrameCore.Runtime
{
    public class OTimerModule : MonoBehaviour
    {
        private List<OTimer> _timers;
        private List<OTimer> _timersToAdd;
        private List<OTimer> _timersToRemove;

        private void Awake()
        {
            _timers = new List<OTimer>();
            _timersToAdd = new List<OTimer>();
            _timersToRemove = new List<OTimer>();
            DontDestroyOnLoad(this);
        }

        public void RegisterTimer(OTimer timer)
        {
            _timersToAdd.Add(timer);
        }

        public void CancelAllTimers()
        {
            foreach (OTimer timer in _timers)
            {
                timer.Cancel();
            }

            _timers = new List<OTimer>();
            _timersToAdd = new List<OTimer>();
        }

        public void PauseAllTimers()
        {
            foreach (OTimer timer in _timers)
            {
                timer.Pause();
            }
        }

        public void ResumeAllTimers()
        {
            foreach (OTimer timer in _timers)
            {
                timer.Resume();
            }
        }

        private void UpdateAllTimers()
        {
            if (_timersToAdd.Count > 0)
            {
                _timers.AddRange(_timersToAdd);
                _timersToAdd.Clear();
            }

            foreach (OTimer timer in _timers)
            {
                if (timer.IsDone)
                {
                    _timersToRemove.Add(timer);
                }
                else
                {
                    timer.Update();
                }
            }

            foreach (var timer in _timersToRemove)
            {
                _timers.Remove(timer);
            }

            _timersToRemove.Clear();
        }

        public void Update()
        {
            this.UpdateAllTimers();
        }
    }
}
