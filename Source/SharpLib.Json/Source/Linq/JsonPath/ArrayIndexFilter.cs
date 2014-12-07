using System.Collections.Generic;
using System.Globalization;

namespace SharpLib.Json.Linq
{
    internal class ArrayIndexFilter : PathFilter
    {
        #region Ñגמיסעגא

        public int? Index { get; set; }

        #endregion

        #region Ìועמהû

        public override IEnumerable<JToken> ExecuteFilter(IEnumerable<JToken> current, bool errorWhenNoMatch)
        {
            foreach (JToken t in current)
            {
                if (Index != null)
                {
                    JToken v = GetTokenIndex(t, errorWhenNoMatch, Index.Value);

                    if (v != null)
                    {
                        yield return v;
                    }
                }
                else
                {
                    if (t is JArray || t is JConstructor)
                    {
                        foreach (JToken v in t)
                        {
                            yield return v;
                        }
                    }
                    else
                    {
                        if (errorWhenNoMatch)
                        {
                            throw new JsonException("Index * not valid on {0}.".FormatWith(CultureInfo.InvariantCulture, t.GetType().Name));
                        }
                    }
                }
            }
        }

        #endregion
    }
}