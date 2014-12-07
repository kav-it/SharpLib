using System;
using System.Globalization;
using System.Runtime.Serialization;

using SharpLib.Json.Linq;

namespace SharpLib.Json
{
    internal class JsonFormatterConverter : IFormatterConverter
    {
        #region Поля

        private readonly JsonSerializer _serializer;

        #endregion

        #region Конструктор

        public JsonFormatterConverter(JsonSerializer serializer)
        {
            ValidationUtils.ArgumentNotNull(serializer, "serializer");

            _serializer = serializer;
        }

        #endregion

        #region Методы

        private T GetTokenValue<T>(object value)
        {
            ValidationUtils.ArgumentNotNull(value, "value");

            JValue v = (JValue)value;
            return (T)System.Convert.ChangeType(v.Value, typeof(T), CultureInfo.InvariantCulture);
        }

        public object Convert(object value, Type type)
        {
            ValidationUtils.ArgumentNotNull(value, "value");

            JToken token = value as JToken;
            if (token == null)
            {
                throw new ArgumentException("Value is not a JToken.", "value");
            }

            return _serializer.Deserialize(token.CreateReader(), type);
        }

        public object Convert(object value, TypeCode typeCode)
        {
            ValidationUtils.ArgumentNotNull(value, "value");

            if (value is JValue)
            {
                value = ((JValue)value).Value;
            }

            return System.Convert.ChangeType(value, typeCode, CultureInfo.InvariantCulture);
        }

        public bool ToBoolean(object value)
        {
            return GetTokenValue<bool>(value);
        }

        public byte ToByte(object value)
        {
            return GetTokenValue<byte>(value);
        }

        public char ToChar(object value)
        {
            return GetTokenValue<char>(value);
        }

        public DateTime ToDateTime(object value)
        {
            return GetTokenValue<DateTime>(value);
        }

        public decimal ToDecimal(object value)
        {
            return GetTokenValue<decimal>(value);
        }

        public double ToDouble(object value)
        {
            return GetTokenValue<double>(value);
        }

        public short ToInt16(object value)
        {
            return GetTokenValue<short>(value);
        }

        public int ToInt32(object value)
        {
            return GetTokenValue<int>(value);
        }

        public long ToInt64(object value)
        {
            return GetTokenValue<long>(value);
        }

        public sbyte ToSByte(object value)
        {
            return GetTokenValue<sbyte>(value);
        }

        public float ToSingle(object value)
        {
            return GetTokenValue<float>(value);
        }

        public string ToString(object value)
        {
            return GetTokenValue<string>(value);
        }

        public ushort ToUInt16(object value)
        {
            return GetTokenValue<ushort>(value);
        }

        public uint ToUInt32(object value)
        {
            return GetTokenValue<uint>(value);
        }

        public ulong ToUInt64(object value)
        {
            return GetTokenValue<ulong>(value);
        }

        #endregion
    }
}