using System;
using System.ComponentModel;
using System.IO;
using System.Text;

namespace SharpLib.Log
{
    [LayoutRenderer("guid")]
    public class GuidLayoutRenderer : LayoutRenderer
    {
        #region Свойства

        [DefaultValue("N")]
        public string Format { get; set; }

        #endregion

        #region Конструктор

        public GuidLayoutRenderer()
        {
            Format = "N";
        }

        #endregion

        #region Методы

        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            builder.Append(Guid.NewGuid().ToString(Format));
        }

        #endregion
    }
}