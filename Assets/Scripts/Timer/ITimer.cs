using System;

namespace Timer
{
    public interface ITimer
    {
        float TimeRemaining { get; }
        bool IsRunning { get; }
        
        event Action<float> OnTimeChanged;
        event Action OnTimerFinished;
        
        void Start(float duration);        
        void Pause();
        void Resume();
        void Tick(float deltaTime);
    }
}
