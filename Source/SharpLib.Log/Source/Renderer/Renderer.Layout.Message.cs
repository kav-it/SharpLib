using System.Text;

namespace SharpLib.Log
{
    [LayoutRenderer("message")]
    [ThreadAgnostic]
    public class MessageLayoutRenderer : LayoutRenderer
    {
        #region Свойства

        public bool WithException { get; set; }

        public string ExceptionSeparator { get; set; }

        #endregion

        #region Конструктор

        public MessageLayoutRenderer()
        {
            ExceptionSeparator = EnvironmentHelper.NewLine;
        }

        #endregion

        #region Методы

        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            builder.Append(logEvent.FormattedMessage);
            if (WithException && logEvent.Exception != null)
            {
                builder.Append(ExceptionSeparator);
                builder.Append(logEvent.Exception);
            }
        }

        #endregion
    }
}