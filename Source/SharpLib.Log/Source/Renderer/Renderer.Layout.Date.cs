using System.ComponentModel;
using System.Globalization;
using System.Text;

namespace SharpLib.Log
{
    [LayoutRenderer("date")]
    [ThreadAgnostic]
    public class DateLayoutRenderer : LayoutRenderer
    {
        #region Свойства

        public CultureInfo Culture { get; set; }

        [DefaultParameter]
        public string Format { get; set; }

        [DefaultValue(false)]
        public bool UniversalTime { get; set; }

        #endregion

        #region Конструктор

        public DateLayoutRenderer()
        {
            Format = @"yyyy-MM-dd HH\:mm\:ss";
            Culture = CultureInfo.InvariantCulture;
        }

        #endregion

        #region Методы

        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            var ts = logEvent.TimeStamp;
            if (UniversalTime)
            {
                ts = ts.ToUniversalTime();
            }

            builder.Append(ts.ToString(Format, Culture));
        }

        #endregion
    }
}