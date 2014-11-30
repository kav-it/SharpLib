using System;
using System.IO;
using System.Text;

namespace SharpLib.Log
{
    [LayoutRenderer("machinename")]
    [AppDomainFixedOutput]
    [ThreadAgnostic]
    public class MachineNameLayoutRenderer : LayoutRenderer
    {
        #region Свойства

        internal string MachineName { get; private set; }

        #endregion

        #region Методы

        protected override void InitializeLayoutRenderer()
        {
            base.InitializeLayoutRenderer();
            try
            {
                MachineName = Environment.MachineName;
            }
            catch (Exception exception)
            {
                if (exception.MustBeRethrown())
                {
                    throw;
                }

                MachineName = string.Empty;
            }
        }

        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            builder.Append(MachineName);
        }

        #endregion
    }
}