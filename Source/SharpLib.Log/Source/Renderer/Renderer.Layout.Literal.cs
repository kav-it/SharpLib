
using System;
using System.Collections.Generic;
using System.Text;

namespace SharpLib.Log
{
    [LayoutRenderer("literal")]
    [ThreadAgnostic]
    [AppDomainFixedOutput]
    public class LiteralLayoutRenderer : LayoutRenderer
    {
        #region Свойства

        public string Text { get; set; }

        #endregion

        #region Конструктор

        public LiteralLayoutRenderer()
        {
        }

        public LiteralLayoutRenderer(string text)
        {
            Text = text;
        }

        #endregion

        #region Методы

        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            builder.Append(Text);
        }

        #endregion
    }
}
