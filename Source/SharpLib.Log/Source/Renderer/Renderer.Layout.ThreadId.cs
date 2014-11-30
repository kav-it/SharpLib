using System.Globalization;
using System.Text;
using System.Threading;

namespace SharpLib.Log
{
    [LayoutRenderer("threadid")]
    public class ThreadIdLayoutRenderer : LayoutRenderer
    {
        #region Методы

        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            builder.Append(Thread.CurrentThread.ManagedThreadId.ToString(CultureInfo.InvariantCulture));
        }

        #endregion
    }
}