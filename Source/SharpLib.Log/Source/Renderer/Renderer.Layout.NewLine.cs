using System.Text;

namespace SharpLib.Log
{
    [LayoutRenderer("newline")]
    public class NewLineLayoutRenderer : LayoutRenderer
    {
        #region Методы

        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            builder.Append(EnvironmentHelper.NewLine);
        }

        #endregion
    }
}