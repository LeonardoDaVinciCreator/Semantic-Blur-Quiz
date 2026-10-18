using System;

namespace RulerPanel
{
    public class RulerPanelModel : IRulerPanel
    {
        public bool IsOpen { get; private set; }
        
        public event Action OnClosed;

        public void Open()
        {
            IsOpen = true;
        }

        public void Close()
        {
            if (!IsOpen) return;
            
            IsOpen = false;
            OnClosed?.Invoke();
        }
    }
}