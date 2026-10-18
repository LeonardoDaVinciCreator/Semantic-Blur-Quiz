using System;

namespace RulerPanel
{
    public interface IRulerPanel
    {
        bool IsOpen { get; }
        event Action OnClosed;
        
        void Open();
        void Close();
    }
}