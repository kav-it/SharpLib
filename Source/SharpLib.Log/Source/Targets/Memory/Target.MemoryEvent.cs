using System;

namespace SharpLib.Log
{
    [Target("MemoryEvent")]
    public sealed class MemoryEventTarget : TargetWithLayout
    {
        #region События

        public MemoryEventTargetHandler OnReceived;

        #endregion

        #region Методы

        public MemoryEventTarget()
        {
        }

        public MemoryEventTarget(string name, MemoryEventTargetHandler handler)
        {
            Name = name;
            OnReceived = handler;
        }

        protected override void Write(LogEventInfo logEvent)
        {
            if (OnReceived != null)
            {
                OnReceived(this, new MemoryEventTargetArgs(logEvent, Layout));
            }
        }

        #endregion
    }
}