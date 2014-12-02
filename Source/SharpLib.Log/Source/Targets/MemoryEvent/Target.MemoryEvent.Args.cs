using System;

namespace SharpLib.Log
{
    public class MemoryEventTargetArgs: EventArgs
    {
        public LogEventInfo Value { get; set; }

        public MemoryEventTargetArgs(LogEventInfo value)
        {
            Value = value;
        }
    }
}