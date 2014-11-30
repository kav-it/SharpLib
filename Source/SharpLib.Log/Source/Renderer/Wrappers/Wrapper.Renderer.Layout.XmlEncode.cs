using System.ComponentModel;
using System.Text;

namespace SharpLib.Log
{
    [LayoutRenderer("xml-encode")]
    [AmbientProperty("XmlEncode")]
    [ThreadAgnostic]
    public sealed class XmlEncodeLayoutRendererWrapper : WrapperLayoutRendererBase
    {
        #region Свойства

        [DefaultValue(true)]
        public bool XmlEncode { get; set; }

        #endregion

        #region Конструктор

        public XmlEncodeLayoutRendererWrapper()
        {
            XmlEncode = true;
        }

        #endregion

        #region Методы

        protected override string Transform(string text)
        {
            return XmlEncode ? DoXmlEscape(text) : text;
        }

        private static string DoXmlEscape(string text)
        {
            var sb = new StringBuilder(text.Length);

            foreach (char t in text)
            {
                switch (t)
                {
                    case '<':
                        sb.Append("&lt;");
                        break;

                    case '>':
                        sb.Append("&gt;");
                        break;

                    case '&':
                        sb.Append("&amp;");
                        break;

                    case '\'':
                        sb.Append("&apos;");
                        break;

                    case '"':
                        sb.Append("&quot;");
                        break;

                    default:
                        sb.Append(t);
                        break;
                }
            }

            return sb.ToString();
        }

        #endregion
    }
}