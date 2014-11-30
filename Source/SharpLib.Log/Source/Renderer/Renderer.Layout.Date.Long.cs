using System;
using System.ComponentModel;
using System.Text;

namespace SharpLib.Log
{
    [LayoutRenderer("longdate")]
    [ThreadAgnostic]
    public class LongDateLayoutRenderer : LayoutRenderer
    {
        #region Свойства

        [DefaultValue(false)]
        public bool UniversalTime { get; set; }

        #endregion

        #region Методы

        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            DateTime dt = logEvent.TimeStamp;
            if (UniversalTime)
            {
                dt = dt.ToUniversalTime();
            }

            builder.Append(dt.Year);
            builder.Append('-');
            Append2DigitsZeroPadded(builder, dt.Month);
            builder.Append('-');
            Append2DigitsZeroPadded(builder, dt.Day);
            builder.Append(' ');
            Append2DigitsZeroPadded(builder, dt.Hour);
            builder.Append(':');
            Append2DigitsZeroPadded(builder, dt.Minute);
            builder.Append(':');
            Append2DigitsZeroPadded(builder, dt.Second);
            builder.Append('.');
            Append3DigitsZeroPadded(builder, (int)(dt.Ticks % 10000000) / 1000);
        }

        private static void Append2DigitsZeroPadded(StringBuilder builder, int number)
        {
            builder.Append((char)((number / 10) + '0'));
            builder.Append((char)((number % 10) + '0'));
        }

        private static void Append3DigitsZeroPadded(StringBuilder builder, int number)
        {
            builder.Append((char)(((number / 100) % 10) + '0'));
            builder.Append((char)(((number / 10) % 10) + '0'));
            builder.Append((char)(((number / 1) % 10) + '0'));
        }

        #endregion
    }
}