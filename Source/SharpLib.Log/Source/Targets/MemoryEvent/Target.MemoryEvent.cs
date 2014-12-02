using System;

namespace SharpLib.Log
{
    [Target("MemoryEvent")]
    public sealed class MemoryEventTarget : TargetWithLayout
    {
        #region События

        public MemoryEventTargetHandler _onReceived;

        #endregion

        #region Методы

        public MemoryEventTarget(string name, MemoryEventTargetHandler handler)
        {
            Name = name;
            _onReceived = handler;
        }

        protected override void Write(LogEventInfo logEvent)
        {
            _onReceived(this, new MemoryEventTargetArgs(logEvent));
        }

        #endregion
    }
}