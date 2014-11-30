using System.Globalization;
using System.Text;

namespace SharpLib.Log
{
    [LayoutRenderer("ticks")]
    [ThreadAgnostic]
    public class TicksLayoutRenderer : LayoutRenderer
    {
        #region Методы

        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            builder.Append(logEvent.TimeStamp.Ticks.ToString(CultureInfo.InvariantCulture));
        }

        #endregion
    }
}