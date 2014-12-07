using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace SharpLib.Json.Linq
{
    public static class Extensions
    {
        #region Методы

        public static IJEnumerable<JToken> Ancestors<T>(this IEnumerable<T> source) where T : JToken
        {
            ValidationUtils.ArgumentNotNull(source, "source");

            return source.SelectMany(j => j.Ancestors()).AsJEnumerable();
        }

        public static IJEnumerable<JToken> Descendants<T>(this IEnumerable<T> source) where T : JContainer
        {
            ValidationUtils.ArgumentNotNull(source, "source");

            return source.SelectMany(j => j.Descendants()).AsJEnumerable();
        }

        public static IJEnumerable<JProperty> Properties(this IEnumerable<JObject> source)
        {
            ValidationUtils.ArgumentNotNull(source, "source");

            return source.SelectMany(d => d.Properties()).AsJEnumerable();
        }

        public static IJEnumerable<JToken> Values(this IEnumerable<JToken> source, object key)
        {
            return Values<JToken, JToken>(source, key).AsJEnumerable();
        }

        public static IJEnumerable<JToken> Values(this IEnumerable<JToken> source)
        {
            return source.Values(null);
        }

        public static IEnumerable<T> Values<T>(this IEnumerable<JToken> source, object key)
        {
            return Values<JToken, T>(source, key);
        }

        public static IEnumerable<T> Values<T>(this IEnumerable<JToken> source)
        {
            return Values<JToken, T>(source, null);
        }

        public static T Value<T>(this IEnumerable<JToken> value)
        {
            return value.Value<JToken, T>();
        }

        public static TU Value<T, TU>(this IEnumerable<T> value) where T : JToken
        {
            ValidationUtils.ArgumentNotNull(value, "source");

            JToken token = value as JToken;
            if (token == null)
            {
                throw new ArgumentException("Source value must be a JToken.");
            }

            return token.Convert<JToken, TU>();
        }

        internal static IEnumerable<TU> Values<T, TU>(this IEnumerable<T> source, object key) where T : JToken
        {
            ValidationUtils.ArgumentNotNull(source, "source");

            foreach (JToken token in source)
            {
                if (key == null)
                {
                    if (token is JValue)
                    {
                        yield return Convert<JValue, TU>((JValue)token);
                    }
                    else
                    {
                        foreach (JToken t in token.Children())
                        {
                            yield return t.Convert<JToken, TU>();
                        }
                    }
                }
                else
                {
                    JToken value = token[key];
                    if (value != null)
                    {
                        yield return value.Convert<JToken, TU>();
                    }
                }
            }
        }

        public static IJEnumerable<JToken> Children<T>(this IEnumerable<T> source) where T : JToken
        {
            return Children<T, JToken>(source).AsJEnumerable();
        }

        public static IEnumerable<TU> Children<T, TU>(this IEnumerable<T> source) where T : JToken
        {
            ValidationUtils.ArgumentNotNull(source, "source");

            return source.SelectMany(c => c.Children()).Convert<JToken, TU>();
        }

        internal static IEnumerable<TU> Convert<T, TU>(this IEnumerable<T> source) where T : JToken
        {
            ValidationUtils.ArgumentNotNull(source, "source");

            return source.Select(Convert<JToken, TU>);
        }

        internal static TU Convert<T, TU>(this T token) where T : JToken
        {
            if (token == null)
            {
                return default(TU);
            }

            if (token is TU
                && typeof(TU) != typeof(IComparable) && typeof(TU) != typeof(IFormattable))
            {
                return (TU)(object)token;
            }
            JValue value = token as JValue;
            if (value == null)
            {
                throw new InvalidCastException("Cannot cast {0} to {1}.".FormatWith(CultureInfo.InvariantCulture, token.GetType(), typeof(T)));
            }

            if (value.Value is TU)
            {
                return (TU)value.Value;
            }

            Type targetType = typeof(TU);

            if (ReflectionUtils.IsNullableType(targetType))
            {
                if (value.Value == null)
                {
                    return default(TU);
                }

                targetType = Nullable.GetUnderlyingType(targetType);
            }

            return (TU)System.Convert.ChangeType(value.Value, targetType, CultureInfo.InvariantCulture);
        }

        public static IJEnumerable<JToken> AsJEnumerable(this IEnumerable<JToken> source)
        {
            return source.AsJEnumerable<JToken>();
        }

        public static IJEnumerable<T> AsJEnumerable<T>(this IEnumerable<T> source) where T : JToken
        {
            if (source == null)
            {
                return null;
            }
            if (source is IJEnumerable<T>)
            {
                return (IJEnumerable<T>)source;
            }
            return new JEnumerable<T>(source);
        }

        #endregion
    }
}