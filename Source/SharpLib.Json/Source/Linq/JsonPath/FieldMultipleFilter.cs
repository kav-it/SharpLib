using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace SharpLib.Json.Linq
{
    internal class FieldMultipleFilter : PathFilter
    {
        #region Ñגמיסעגא

        public List<string> Names { get; set; }

        #endregion

        #region Ìועמהû

        public override IEnumerable<JToken> ExecuteFilter(IEnumerable<JToken> current, bool errorWhenNoMatch)
        {
            foreach (JToken t in current)
            {
                JObject o = t as JObject;
                if (o != null)
                {
                    foreach (string name in Names)
                    {
                        JToken v = o[name];

                        if (v != null)
                        {
                            yield return v;
                        }

                        if (errorWhenNoMatch)
                        {
                            throw new JsonException("Property '{0}' does not exist on JObject.".FormatWith(CultureInfo.InvariantCulture, name));
                        }
                    }
                }
                else
                {
                    if (errorWhenNoMatch)
                    {
                        throw new JsonException("Properties {0} not valid on {1}.".FormatWith(CultureInfo.InvariantCulture, string.Join(", ", Names.Select(n => "'" + n + "'").ToArray()),
                            t.GetType().Name));
                    }
                }
            }
        }

        #endregion
    }
}