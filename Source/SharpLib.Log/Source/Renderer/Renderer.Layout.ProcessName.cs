using System.ComponentModel;
using System.Text;

namespace SharpLib.Log
{
    [LayoutRenderer("processname")]
    [AppDomainFixedOutput]
    [ThreadAgnostic]
    public class ProcessNameLayoutRenderer : LayoutRenderer
    {
        #region Свойства

        [DefaultValue(false)]
        public bool FullName { get; set; }

        #endregion

        #region Методы

        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            builder.Append(FullName
                ? ThreadIdHelper.Instance.CurrentProcessName
                : ThreadIdHelper.Instance.CurrentProcessBaseName);
        }

        #endregion
    }
}