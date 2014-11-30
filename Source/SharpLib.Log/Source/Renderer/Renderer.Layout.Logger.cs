using System;
using System.ComponentModel;
using System.IO;
using System.Text;

namespace SharpLib.Log
{
    [LayoutRenderer("logger")]
    [ThreadAgnostic]
    public class LoggerLayoutRenderer : LayoutRenderer
    {
        #region Свойства

        [DefaultValue(false)]
        public bool ShortName { get; set; }

        #endregion

        #region Методы

        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            if (ShortName)
            {
                int lastDot = logEvent.LoggerName.LastIndexOf('.');
                if (lastDot < 0)
                {
                    builder.Append(logEvent.LoggerName);
                }
                else
                {
                    builder.Append(logEvent.LoggerName.Substring(lastDot + 1));
                }
            }
            else
            {
                builder.Append(logEvent.LoggerName);
            }
        }

        #endregion
    }
}