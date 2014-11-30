using System.Text;

namespace SharpLib.Log
{
    [LayoutRenderer("level")]
    [ThreadAgnostic]
    public class LevelLayoutRenderer : LayoutRenderer
    {
        #region Методы

        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            builder.Append(logEvent.Level);
        }

        #endregion
    }
}