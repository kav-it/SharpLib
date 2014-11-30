using System.Text;

namespace SharpLib.Log
{
    [LayoutRenderer("threadname")]
    public class ThreadNameLayoutRenderer : LayoutRenderer
    {
        #region Методы

        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            builder.Append(System.Threading.Thread.CurrentThread.Name);
        }

        #endregion
    }
}