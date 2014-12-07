using System.Collections.Generic;
using System.Globalization;

namespace SharpLib.Json.Linq
{
    internal abstract class PathFilter
    {
        #region Ìועמהû

        public abstract IEnumerable<JToken> ExecuteFilter(IEnumerable<JToken> current, bool errorWhenNoMatch);

        protected static JToken GetTokenIndex(JToken t, bool errorWhenNoMatch, int index)
        {
            JArray a = t as JArray;
            JConstructor c = t as JConstructor;

            if (a != null)
            {
                if (a.Count <= index)
                {
                    if (errorWhenNoMatch)
                    {
                        throw new JsonException("Index {0} outside the bounds of JArray.".FormatWith(CultureInfo.InvariantCulture, index));
                    }

                    return null;
                }

                return a[index];
            }
            if (c != null)
            {
                if (c.Count <= index)
                {
                    if (errorWhenNoMatch)
                    {
                        throw new JsonException("Index {0} outside the bounds of JConstructor.".FormatWith(CultureInfo.InvariantCulture, index));
                    }

                    return null;
                }

                return c[index];
            }
            if (errorWhenNoMatch)
            {
                throw new JsonException("Index {0} not valid on {1}.".FormatWith(CultureInfo.InvariantCulture, index, t.GetType().Name));
            }

            return null;
        }

        #endregion
    }
}