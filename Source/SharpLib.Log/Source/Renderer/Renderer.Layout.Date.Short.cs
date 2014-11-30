using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Text;

namespace SharpLib.Log
{
    [LayoutRenderer("shortdate")]
    [ThreadAgnostic]
    public class ShortDateLayoutRenderer : LayoutRenderer
    {
        #region Свойства

        [DefaultValue(false)]
        public bool UniversalTime { get; set; }

        #endregion

        #region Методы

        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            var ts = logEvent.TimeStamp;
            if (UniversalTime)
            {
                ts = ts.ToUniversalTime();
            }

            builder.Append(ts.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));
        }

        #endregion
    }
}