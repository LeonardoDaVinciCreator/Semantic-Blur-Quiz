using System;
using Timer;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

namespace Core
{
    public class TimerController : MonoBehaviour
    {        
        [SerializeField] private TimerView _timerView;
        [SerializeField] private float _duration = 60f;

        //[SerializeField] private QuizGameController _quizGameController;

        private ITimer _timer;        
        private string _timerTextMask;

        public event Action OnTimerFinished;        

        private void Awake()
        {
            _timer = new TimerModel();
            _timer.OnTimeChanged += UpdateDisplay;
            _timer.OnTimerFinished += HandleTimerFinished;
        }

        private void OnDestroy()
        {
            if (_timer == null) return;

            _timer.OnTimeChanged -= UpdateDisplay;
            _timer.OnTimerFinished -= HandleTimerFinished;
        }

        public void StartTimer()
        {
            _timer.Start(_duration);
        }

        private void Update()
        {
            _timer.Tick(Time.deltaTime);
        }

        private void UpdateDisplay(float time)
        {
            if (_timerView == null) return;
            if (string.IsNullOrEmpty(_timerTextMask)) return;

            _timerView.SetText(string.Format(_timerTextMask, Mathf.Ceil(time)));
        }

        private void HandleTimerFinished()
        {
            OnTimerFinished?.Invoke();
        }        

        public void PauseTimer() => _timer.Pause();

        public void UpdateTextMask(string timerTextMask)
        {
            _timerTextMask = timerTextMask;
            UpdateDisplay(_timer.TimeRemaining);
        }

        public void SetVisible(bool visible)
        {
            if (_timerView == null) return;

            _timerView.SetVisible(visible);

            if (!visible)
                _timer.Pause();
        }
        
    }
}