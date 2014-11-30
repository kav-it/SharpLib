using System;
using System.ComponentModel;
using System.Text;

namespace SharpLib.Log
{
    [LayoutRenderer("json-encode")]
    [AmbientProperty("JsonEncode")]
    [ThreadAgnostic]
    public sealed class JsonEncodeLayoutRendererWrapper : WrapperLayoutRendererBase
    {
        #region Свойства

        [DefaultValue(true)]
        public bool JsonEncode { get; set; }

        #endregion

        #region Конструктор

        public JsonEncodeLayoutRendererWrapper()
        {
            JsonEncode = true;
        }

        #endregion

        #region Методы

        protected override string Transform(string text)
        {
            return JsonEncode ? DoJsonEscape(text) : text;
        }

        private static string DoJsonEscape(string text)
        {
            var sb = new StringBuilder(text.Length);

            foreach (char ch in text)
            {
                switch (ch)
                {
                    case '"':
                        sb.Append("\\\"");
                        break;

                    case '\\':
                        sb.Append("\\\\");
                        break;

                    case '/':
                        sb.Append("\\/");
                        break;

                    case '\b':
                        sb.Append("\\b");
                        break;

                    case '\r':
                        sb.Append("\\r");
                        break;

                    case '\n':
                        sb.Append("\\n");
                        break;

                    case '\f':
                        sb.Append("\\f");
                        break;

                    case '\t':
                        sb.Append("\\t");
                        break;

                    default:
                        if (NeedsEscaping(ch))
                        {
                            sb.Append("\\u");
                            sb.Append(Convert.ToString(ch, 16).PadLeft(4, '0'));
                        }
                        else
                        {
                            sb.Append(ch);
                        }

                        break;
                }
            }

            return sb.ToString();
        }

        private static bool NeedsEscaping(char ch)
        {
            return ch < 32 || ch > 127;
        }

        #endregion
    }
}