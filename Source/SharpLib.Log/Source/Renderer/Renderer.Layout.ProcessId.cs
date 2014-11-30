using System.Globalization;
using System.Text;

namespace SharpLib.Log
{
    [LayoutRenderer("processid")]
    [AppDomainFixedOutput]
    [ThreadAgnostic]
    public class ProcessIdLayoutRenderer : LayoutRenderer
    {
        #region Методы

        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            builder.Append(ThreadIdHelper.Instance.CurrentProcessId.ToString(CultureInfo.InvariantCulture));
        }

        #endregion
    }
}