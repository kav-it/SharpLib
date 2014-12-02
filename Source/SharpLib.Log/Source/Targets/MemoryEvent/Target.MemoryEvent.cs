using System;

namespace SharpLib.Log
{
    [Target("MemoryEvent")]
    public sealed class MemoryEventTarget : TargetWithLayout
    {
        #region �������

        public MemoryEventTargetHandler _onReceived;

        #endregion

        #region ������

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