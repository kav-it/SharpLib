#region License

#endregion

using System.Globalization;
using System.IO;

namespace SharpLib.Json.Linq
{
    public class JRaw : JValue
    {
        #region Конструктор

        public JRaw(JRaw other)
            : base(other)
        {
        }

        public JRaw(object rawJson)
            : base(rawJson, JTokenType.Raw)
        {
        }

        #endregion

        #region Методы

        public static JRaw Create(JsonReader reader)
        {
            using (StringWriter sw = new StringWriter(CultureInfo.InvariantCulture))
            {
                using (JsonTextWriter jsonWriter = new JsonTextWriter(sw))
                {
                    jsonWriter.WriteToken(reader);

                    return new JRaw(sw.ToString());
                }
            }
        }

        internal override JToken CloneToken()
        {
            return new JRaw(this);
        }

        #endregion
    }
}