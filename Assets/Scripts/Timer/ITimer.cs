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
        void Tick(float deltaTime);
    }
}
