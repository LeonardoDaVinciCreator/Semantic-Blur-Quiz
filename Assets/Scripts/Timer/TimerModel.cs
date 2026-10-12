using System;
using Unity.VisualScripting;
using UnityEngine;

namespace Timer
{
    public class TimerModel : ITimer
    {
        private float _timeLeft;
        private bool _isRunning;               

        public float TimeRemaining => Mathf.Max(0f, _timeLeft);
        public bool IsRunning => _isRunning;

        public event Action<float> OnTimeChanged;
        public event Action OnTimerFinished;

        public void Start(float duration)
        {            
            _timeLeft = duration;
            _isRunning = true;

            OnTimeChanged?.Invoke(_timeLeft);            
        }

        public void Pause()
        {
            _isRunning = false;            
        }

        public void Resume()
        {
            if (_timeLeft > 0f)
                _isRunning = true;
        }

        public void Tick(float deltaTime)
        {
            if (!_isRunning) return;

            _timeLeft = Mathf.Max(0f, _timeLeft - deltaTime);
            OnTimeChanged?.Invoke(_timeLeft);

            if (_timeLeft <= 0f)
            {
                _isRunning = false;
                OnTimerFinished?.Invoke();
            }
        }
    }
}
