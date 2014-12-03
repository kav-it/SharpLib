using System;

namespace SharpLib.Log
{
    public class MemoryEventTargetArgs: EventArgs
    {
        public LogEventInfo Value { get; set; }

        public Layout Layout { get; set; }

        public MemoryEventTargetArgs(LogEventInfo value, Layout layout)
        {
            Value = value;
            Layout = layout;
        }
    }
}