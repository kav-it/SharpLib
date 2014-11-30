using System.ComponentModel;
using System.Diagnostics;
using System.Text;

namespace SharpLib.Log
{
    [LayoutRenderer("stacktrace")]
    [ThreadAgnostic]
    public class StackTraceLayoutRenderer : LayoutRenderer, IUsesStackTrace
    {
        #region Свойства

        [DefaultValue("Flat")]
        public StackTraceFormat Format { get; set; }

        [DefaultValue(3)]
        public int TopFrames { get; set; }

        [DefaultValue(" => ")]
        public string Separator { get; set; }

        StackTraceUsage IUsesStackTrace.StackTraceUsage
        {
            get { return StackTraceUsage.WithoutSource; }
        }

        #endregion

        #region Конструктор

        public StackTraceLayoutRenderer()
        {
            Separator = " => ";
            TopFrames = 3;
            Format = StackTraceFormat.Flat;
        }

        #endregion

        #region Методы

        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            bool first = true;
            int startingFrame = logEvent.UserStackFrameNumber + TopFrames - 1;
            if (startingFrame >= logEvent.StackTrace.FrameCount)
            {
                startingFrame = logEvent.StackTrace.FrameCount - 1;
            }

            switch (Format)
            {
                case StackTraceFormat.Raw:
                    for (int i = startingFrame; i >= logEvent.UserStackFrameNumber; --i)
                    {
                        StackFrame f = logEvent.StackTrace.GetFrame(i);
                        builder.Append(f);
                    }

                    break;

                case StackTraceFormat.Flat:
                    for (int i = startingFrame; i >= logEvent.UserStackFrameNumber; --i)
                    {
                        StackFrame f = logEvent.StackTrace.GetFrame(i);
                        if (!first)
                        {
                            builder.Append(Separator);
                        }

                        var type = f.GetMethod().DeclaringType;

                        if (type != null)
                        {
                            builder.Append(type.Name);
                        }
                        else
                        {
                            builder.Append("<no type>");
                        }

                        builder.Append(".");
                        builder.Append(f.GetMethod().Name);
                        first = false;
                    }

                    break;

                case StackTraceFormat.DetailedFlat:
                    for (int i = startingFrame; i >= logEvent.UserStackFrameNumber; --i)
                    {
                        StackFrame f = logEvent.StackTrace.GetFrame(i);
                        if (!first)
                        {
                            builder.Append(Separator);
                        }

                        builder.Append("[");
                        builder.Append(f.GetMethod());
                        builder.Append("]");
                        first = false;
                    }

                    break;
            }
        }

        #endregion
    }
}