using System.Text;

namespace SharpLib.Log
{
    [LayoutRenderer("environment")]
    public class EnvironmentLayoutRenderer : LayoutRenderer
    {
        #region Свойства

        [RequiredParameter]
        [DefaultParameter]
        public string Variable { get; set; }

        public string Default { get; set; }

        #endregion

        #region Методы

        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            if (Variable != null)
            {
                var environmentVariable = EnvironmentHelper.GetSafeEnvironmentVariable(Variable);
                if (!string.IsNullOrEmpty(environmentVariable))
                {
                    var layout = new SimpleLayout(environmentVariable);
                    builder.Append(layout.Render(logEvent));
                }
                else
                {
                    if (Default != null)
                    {
                        var layout = new SimpleLayout(Default);
                        builder.Append(layout.Render(logEvent));
                    }
                }
            }
        }

        #endregion
    }
}