using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.Linq.Expressions;
using System.Numerics;

namespace SharpLib.Json.Linq
{
    public class JValue : JToken, IEquatable<JValue>, IFormattable, IComparable, IComparable<JValue>, IConvertible
    {
        #region Поля

        private object _value;

        private JTokenType _valueType;

        #endregion

        #region Свойства

        public override bool HasValues
        {
            get { return false; }
        }

        public override JTokenType Type
        {
            get { return _valueType; }
        }

        public object Value
        {
            get { return _value; }
            set
            {
                Type currentType = (_value != null) ? _value.GetType() : null;
                Type newType = (value != null) ? value.GetType() : null;

                if (currentType != newType)
                {
                    _valueType = GetValueType(_valueType, value);
                }

                _value = value;
            }
        }

        #endregion

        #region Конструктор

        internal JValue(object value, JTokenType type)
        {
            _value = value;
            _valueType = type;
        }

        public JValue(JValue other)
            : this(other.Value, other.Type)
        {
        }

        public JValue(long value)
            : this(value, JTokenType.Integer)
        {
        }

        public JValue(decimal value)
            : this(value, JTokenType.Float)
        {
        }

        public JValue(char value)
            : this(value, JTokenType.String)
        {
        }

        [CLSCompliant(false)]
        public JValue(ulong value)
            : this(value, JTokenType.Integer)
        {
        }

        public JValue(double value)
            : this(value, JTokenType.Float)
        {
        }

        public JValue(float value)
            : this(value, JTokenType.Float)
        {
        }

        public JValue(DateTime value)
            : this(value, JTokenType.Date)
        {
        }

        public JValue(DateTimeOffset value)
            : this(value, JTokenType.Date)
        {
        }

        public JValue(bool value)
            : this(value, JTokenType.Boolean)
        {
        }

        public JValue(string value)
            : this(value, JTokenType.String)
        {
        }

        public JValue(Guid value)
            : this(value, JTokenType.Guid)
        {
        }

        public JValue(Uri value)
            : this(value, (value != null) ? JTokenType.Uri : JTokenType.Null)
        {
        }

        public JValue(TimeSpan value)
            : this(value, JTokenType.TimeSpan)
        {
        }

        public JValue(object value)
            : this(value, GetValueType(null, value))
        {
        }

        #endregion

        #region Методы

        internal override bool DeepEquals(JToken node)
        {
            JValue other = node as JValue;
            if (other == null)
            {
                return false;
            }
            if (other == this)
            {
                return true;
            }

            return ValuesEquals(this, other);
        }

        private static int CompareBigInteger(BigInteger i1, object i2)
        {
            int result = i1.CompareTo(ConvertUtils.ToBigInteger(i2));

            if (result != 0)
            {
                return result;
            }

            if (i2 is decimal)
            {
                decimal d = (decimal)i2;
                return (0m).CompareTo(Math.Abs(d - Math.Truncate(d)));
            }
            if (i2 is double || i2 is float)
            {
                double d = Convert.ToDouble(i2, CultureInfo.InvariantCulture);
                return (0d).CompareTo(Math.Abs(d - Math.Truncate(d)));
            }

            return result;
        }

        internal static int Compare(JTokenType valueType, object objA, object objB)
        {
            if (objA == null && objB == null)
            {
                return 0;
            }
            if (objA != null && objB == null)
            {
                return 1;
            }
            if (objA == null && objB != null)
            {
                return -1;
            }

            switch (valueType)
            {
                case JTokenType.Integer:
                    if (objA is BigInteger)
                    {
                        return CompareBigInteger((BigInteger)objA, objB);
                    }
                    if (objB is BigInteger)
                    {
                        return -CompareBigInteger((BigInteger)objB, objA);
                    }
                    if (objA is ulong || objB is ulong || objA is decimal || objB is decimal)
                    {
                        return Convert.ToDecimal(objA, CultureInfo.InvariantCulture).CompareTo(Convert.ToDecimal(objB, CultureInfo.InvariantCulture));
                    }
                    if (objA is float || objB is float || objA is double || objB is double)
                    {
                        return CompareFloat(objA, objB);
                    }
                    return Convert.ToInt64(objA, CultureInfo.InvariantCulture).CompareTo(Convert.ToInt64(objB, CultureInfo.InvariantCulture));
                case JTokenType.Float:
                    if (objA is BigInteger)
                    {
                        return CompareBigInteger((BigInteger)objA, objB);
                    }
                    if (objB is BigInteger)
                    {
                        return -CompareBigInteger((BigInteger)objB, objA);
                    }
                    return CompareFloat(objA, objB);
                case JTokenType.Comment:
                case JTokenType.String:
                case JTokenType.Raw:
                    string s1 = Convert.ToString(objA, CultureInfo.InvariantCulture);
                    string s2 = Convert.ToString(objB, CultureInfo.InvariantCulture);

                    return string.CompareOrdinal(s1, s2);
                case JTokenType.Boolean:
                    bool b1 = Convert.ToBoolean(objA, CultureInfo.InvariantCulture);
                    bool b2 = Convert.ToBoolean(objB, CultureInfo.InvariantCulture);

                    return b1.CompareTo(b2);
                case JTokenType.Date:
                    if (objA is DateTime)
                    {
                        DateTime date1 = (DateTime)objA;
                        DateTime date2;

                        if (objB is DateTimeOffset)
                        {
                            date2 = ((DateTimeOffset)objB).DateTime;
                        }
                        else
                        {
                            date2 = Convert.ToDateTime(objB, CultureInfo.InvariantCulture);
                        }

                        return date1.CompareTo(date2);
                    }
                    else
                    {
                        DateTimeOffset date1 = (DateTimeOffset)objA;
                        DateTimeOffset date2;

                        if (objB is DateTimeOffset)
                        {
                            date2 = (DateTimeOffset)objB;
                        }
                        else
                        {
                            date2 = new DateTimeOffset(Convert.ToDateTime(objB, CultureInfo.InvariantCulture));
                        }

                        return date1.CompareTo(date2);
                    }
                case JTokenType.Bytes:
                    if (!(objB is byte[]))
                    {
                        throw new ArgumentException("Object must be of type byte[].");
                    }

                    byte[] bytes1 = objA as byte[];
                    byte[] bytes2 = objB as byte[];
                    if (bytes1 == null)
                    {
                        return -1;
                    }
                    if (bytes2 == null)
                    {
                        return 1;
                    }

                    return MiscellaneousUtils.ByteArrayCompare(bytes1, bytes2);
                case JTokenType.Guid:
                    if (!(objB is Guid))
                    {
                        throw new ArgumentException("Object must be of type Guid.");
                    }

                    Guid guid1 = (Guid)objA;
                    Guid guid2 = (Guid)objB;

                    return guid1.CompareTo(guid2);
                case JTokenType.Uri:
                    if (!(objB is Uri))
                    {
                        throw new ArgumentException("Object must be of type Uri.");
                    }

                    Uri uri1 = (Uri)objA;
                    Uri uri2 = (Uri)objB;

                    return Comparer<string>.Default.Compare(uri1.ToString(), uri2.ToString());
                case JTokenType.TimeSpan:
                    if (!(objB is TimeSpan))
                    {
                        throw new ArgumentException("Object must be of type TimeSpan.");
                    }

                    TimeSpan ts1 = (TimeSpan)objA;
                    TimeSpan ts2 = (TimeSpan)objB;

                    return ts1.CompareTo(ts2);
                default:
                    throw MiscellaneousUtils.CreateArgumentOutOfRangeException("valueType", valueType, "Unexpected value type: {0}".FormatWith(CultureInfo.InvariantCulture, valueType));
            }
        }

        private static int CompareFloat(object objA, object objB)
        {
            double d1 = Convert.ToDouble(objA, CultureInfo.InvariantCulture);
            double d2 = Convert.ToDouble(objB, CultureInfo.InvariantCulture);

            if (MathUtils.ApproxEquals(d1, d2))
            {
                return 0;
            }

            return d1.CompareTo(d2);
        }

        private static bool Operation(ExpressionType operation, object objA, object objB, out object result)
        {
            if (objA is string || objB is string)
            {
                if (operation == ExpressionType.Add || operation == ExpressionType.AddAssign)
                {
                    result = ((objA != null) ? objA.ToString() : null) + ((objB != null) ? objB.ToString() : null);
                    return true;
                }
            }

            if (objA is BigInteger || objB is BigInteger)
            {
                if (objA == null || objB == null)
                {
                    result = null;
                    return true;
                }

                BigInteger i1 = ConvertUtils.ToBigInteger(objA);
                BigInteger i2 = ConvertUtils.ToBigInteger(objB);

                switch (operation)
                {
                    case ExpressionType.Add:
                    case ExpressionType.AddAssign:
                        result = i1 + i2;
                        return true;
                    case ExpressionType.Subtract:
                    case ExpressionType.SubtractAssign:
                        result = i1 - i2;
                        return true;
                    case ExpressionType.Multiply:
                    case ExpressionType.MultiplyAssign:
                        result = i1 * i2;
                        return true;
                    case ExpressionType.Divide:
                    case ExpressionType.DivideAssign:
                        result = i1 / i2;
                        return true;
                }
            }
            else if (objA is ulong || objB is ulong || objA is decimal || objB is decimal)
            {
                if (objA == null || objB == null)
                {
                    result = null;
                    return true;
                }

                decimal d1 = Convert.ToDecimal(objA, CultureInfo.InvariantCulture);
                decimal d2 = Convert.ToDecimal(objB, CultureInfo.InvariantCulture);

                switch (operation)
                {
                    case ExpressionType.Add:
                    case ExpressionType.AddAssign:
                        result = d1 + d2;
                        return true;
                    case ExpressionType.Subtract:
                    case ExpressionType.SubtractAssign:
                        result = d1 - d2;
                        return true;
                    case ExpressionType.Multiply:
                    case ExpressionType.MultiplyAssign:
                        result = d1 * d2;
                        return true;
                    case ExpressionType.Divide:
                    case ExpressionType.DivideAssign:
                        result = d1 / d2;
                        return true;
                }
            }
            else if (objA is float || objB is float || objA is double || objB is double)
            {
                if (objA == null || objB == null)
                {
                    result = null;
                    return true;
                }

                double d1 = Convert.ToDouble(objA, CultureInfo.InvariantCulture);
                double d2 = Convert.ToDouble(objB, CultureInfo.InvariantCulture);

                switch (operation)
                {
                    case ExpressionType.Add:
                    case ExpressionType.AddAssign:
                        result = d1 + d2;
                        return true;
                    case ExpressionType.Subtract:
                    case ExpressionType.SubtractAssign:
                        result = d1 - d2;
                        return true;
                    case ExpressionType.Multiply:
                    case ExpressionType.MultiplyAssign:
                        result = d1 * d2;
                        return true;
                    case ExpressionType.Divide:
                    case ExpressionType.DivideAssign:
                        result = d1 / d2;
                        return true;
                }
            }
            else if (objA is int || objA is uint || objA is long || objA is short || objA is ushort || objA is sbyte || objA is byte ||
                     objB is int || objB is uint || objB is long || objB is short || objB is ushort || objB is sbyte || objB is byte)
            {
                if (objA == null || objB == null)
                {
                    result = null;
                    return true;
                }

                long l1 = Convert.ToInt64(objA, CultureInfo.InvariantCulture);
                long l2 = Convert.ToInt64(objB, CultureInfo.InvariantCulture);

                switch (operation)
                {
                    case ExpressionType.Add:
                    case ExpressionType.AddAssign:
                        result = l1 + l2;
                        return true;
                    case ExpressionType.Subtract:
                    case ExpressionType.SubtractAssign:
                        result = l1 - l2;
                        return true;
                    case ExpressionType.Multiply:
                    case ExpressionType.MultiplyAssign:
                        result = l1 * l2;
                        return true;
                    case ExpressionType.Divide:
                    case ExpressionType.DivideAssign:
                        result = l1 / l2;
                        return true;
                }
            }

            result = null;
            return false;
        }

        internal override JToken CloneToken()
        {
            return new JValue(this);
        }

        public static JValue CreateComment(string value)
        {
            return new JValue(value, JTokenType.Comment);
        }

        public static JValue CreateString(string value)
        {
            return new JValue(value, JTokenType.String);
        }

        public static JValue CreateNull()
        {
            return new JValue(null, JTokenType.Null);
        }

        public static JValue CreateUndefined()
        {
            return new JValue(null, JTokenType.Undefined);
        }

        private static JTokenType GetValueType(JTokenType? current, object value)
        {
            if (value == null)
            {
                return JTokenType.Null;
            }
            if (value == DBNull.Value)
            {
                return JTokenType.Null;
            }
            if (value is string)
            {
                return GetStringValueType(current);
            }
            if (value is long || value is int || value is short || value is sbyte
                || value is ulong || value is uint || value is ushort || value is byte)
            {
                return JTokenType.Integer;
            }
            if (value is Enum)
            {
                return JTokenType.Integer;
            }
            if (value is BigInteger)
            {
                return JTokenType.Integer;
            }
            if (value is double || value is float || value is decimal)
            {
                return JTokenType.Float;
            }
            if (value is DateTime)
            {
                return JTokenType.Date;
            }
            if (value is DateTimeOffset)
            {
                return JTokenType.Date;
            }
            if (value is byte[])
            {
                return JTokenType.Bytes;
            }
            if (value is bool)
            {
                return JTokenType.Boolean;
            }
            if (value is Guid)
            {
                return JTokenType.Guid;
            }
            if (value is Uri)
            {
                return JTokenType.Uri;
            }
            if (value is TimeSpan)
            {
                return JTokenType.TimeSpan;
            }

            throw new ArgumentException("Could not determine JSON object type for type {0}.".FormatWith(CultureInfo.InvariantCulture, value.GetType()));
        }

        private static JTokenType GetStringValueType(JTokenType? current)
        {
            if (current == null)
            {
                return JTokenType.String;
            }

            switch (current.Value)
            {
                case JTokenType.Comment:
                case JTokenType.String:
                case JTokenType.Raw:
                    return current.Value;
                default:
                    return JTokenType.String;
            }
        }

        public override void WriteTo(JsonWriter writer, params JsonConverter[] converters)
        {
            if (converters != null && converters.Length > 0 && _value != null)
            {
                JsonConverter matchingConverter = JsonSerializer.GetMatchingConverter(converters, _value.GetType());
                if (matchingConverter != null && matchingConverter.CanWrite)
                {
                    matchingConverter.WriteJson(writer, _value, JsonSerializer.CreateDefault());
                    return;
                }
            }

            switch (_valueType)
            {
                case JTokenType.Comment:
                    writer.WriteComment((_value != null) ? _value.ToString() : null);
                    return;
                case JTokenType.Raw:
                    writer.WriteRawValue((_value != null) ? _value.ToString() : null);
                    return;
                case JTokenType.Null:
                    writer.WriteNull();
                    return;
                case JTokenType.Undefined:
                    writer.WriteUndefined();
                    return;
                case JTokenType.Integer:
                    if (_value is BigInteger)
                    {
                        writer.WriteValue((BigInteger)_value);
                    }
                    else
                    {
                        writer.WriteValue(Convert.ToInt64(_value, CultureInfo.InvariantCulture));
                    }
                    return;
                case JTokenType.Float:
                    if (_value is decimal)
                    {
                        writer.WriteValue((decimal)_value);
                    }
                    else if (_value is double)
                    {
                        writer.WriteValue((double)_value);
                    }
                    else if (_value is float)
                    {
                        writer.WriteValue((float)_value);
                    }
                    else
                    {
                        writer.WriteValue(Convert.ToDouble(_value, CultureInfo.InvariantCulture));
                    }
                    return;
                case JTokenType.String:
                    writer.WriteValue((_value != null) ? _value.ToString() : null);
                    return;
                case JTokenType.Boolean:
                    writer.WriteValue(Convert.ToBoolean(_value, CultureInfo.InvariantCulture));
                    return;
                case JTokenType.Date:
                    if (_value is DateTimeOffset)
                    {
                        writer.WriteValue((DateTimeOffset)_value);
                    }
                    else
                    {
                        writer.WriteValue(Convert.ToDateTime(_value, CultureInfo.InvariantCulture));
                    }
                    return;
                case JTokenType.Bytes:
                    writer.WriteValue((byte[])_value);
                    return;
                case JTokenType.Guid:
                case JTokenType.Uri:
                case JTokenType.TimeSpan:
                    writer.WriteValue((_value != null) ? _value.ToString() : null);
                    return;
            }

            throw MiscellaneousUtils.CreateArgumentOutOfRangeException("TokenType", _valueType, "Unexpected token type.");
        }

        internal override int GetDeepHashCode()
        {
            int valueHashCode = (_value != null) ? _value.GetHashCode() : 0;

            return ((int)_valueType).GetHashCode() ^ valueHashCode;
        }

        private static bool ValuesEquals(JValue v1, JValue v2)
        {
            return (v1 == v2 || (v1._valueType == v2._valueType && Compare(v1._valueType, v1._value, v2._value) == 0));
        }

        public bool Equals(JValue other)
        {
            if (other == null)
            {
                return false;
            }

            return ValuesEquals(this, other);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            JValue otherValue = obj as JValue;
            if (otherValue != null)
            {
                return Equals(otherValue);
            }

            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            if (_value == null)
            {
                return 0;
            }

            return _value.GetHashCode();
        }

        public override string ToString()
        {
            if (_value == null)
            {
                return string.Empty;
            }

            return _value.ToString();
        }

        public string ToString(string format)
        {
            return ToString(format, CultureInfo.CurrentCulture);
        }

        public string ToString(IFormatProvider formatProvider)
        {
            return ToString(null, formatProvider);
        }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            if (_value == null)
            {
                return string.Empty;
            }

            IFormattable formattable = _value as IFormattable;
            if (formattable != null)
            {
                return formattable.ToString(format, formatProvider);
            }
            return _value.ToString();
        }

        protected override DynamicMetaObject GetMetaObject(Expression parameter)
        {
            return new DynamicProxyMetaObject<JValue>(parameter, this, new JValueDynamicProxy(), true);
        }

        int IComparable.CompareTo(object obj)
        {
            if (obj == null)
            {
                return 1;
            }

            object otherValue = (obj is JValue) ? ((JValue)obj).Value : obj;

            return Compare(_valueType, _value, otherValue);
        }

        public int CompareTo(JValue obj)
        {
            if (obj == null)
            {
                return 1;
            }

            return Compare(_valueType, _value, obj._value);
        }

        TypeCode IConvertible.GetTypeCode()
        {
            if (_value == null)
            {
                return TypeCode.Empty;
            }

            if (_value is DateTimeOffset)
            {
                return TypeCode.DateTime;
            }
            if (_value is BigInteger)
            {
                return TypeCode.Object;
            }
            return System.Type.GetTypeCode(_value.GetType());
        }

        bool IConvertible.ToBoolean(IFormatProvider provider)
        {
            return (bool)this;
        }

        char IConvertible.ToChar(IFormatProvider provider)
        {
            return (char)this;
        }

        sbyte IConvertible.ToSByte(IFormatProvider provider)
        {
            return (sbyte)this;
        }

        byte IConvertible.ToByte(IFormatProvider provider)
        {
            return (byte)this;
        }

        short IConvertible.ToInt16(IFormatProvider provider)
        {
            return (short)this;
        }

        ushort IConvertible.ToUInt16(IFormatProvider provider)
        {
            return (ushort)this;
        }

        int IConvertible.ToInt32(IFormatProvider provider)
        {
            return (int)this;
        }

        uint IConvertible.ToUInt32(IFormatProvider provider)
        {
            return (uint)this;
        }

        long IConvertible.ToInt64(IFormatProvider provider)
        {
            return (long)this;
        }

        ulong IConvertible.ToUInt64(IFormatProvider provider)
        {
            return (ulong)this;
        }

        float IConvertible.ToSingle(IFormatProvider provider)
        {
            return (float)this;
        }

        double IConvertible.ToDouble(IFormatProvider provider)
        {
            return (double)this;
        }

        decimal IConvertible.ToDecimal(IFormatProvider provider)
        {
            return (decimal)this;
        }

        DateTime IConvertible.ToDateTime(IFormatProvider provider)
        {
            return (DateTime)this;
        }

        object IConvertible.ToType(Type conversionType, IFormatProvider provider)
        {
            return ToObject(conversionType);
        }

        #endregion

        #region Вложенный класс: JValueDynamicProxy

        private class JValueDynamicProxy : DynamicProxy<JValue>
        {
            #region Методы

            public override bool TryConvert(JValue instance, ConvertBinder binder, out object result)
            {
                if (binder.Type == typeof(JValue))
                {
                    result = instance;
                    return true;
                }

                object value = instance.Value;

                if (value == null)
                {
                    result = null;
                    return ReflectionUtils.IsNullable(binder.Type);
                }

                result = ConvertUtils.Convert(value, CultureInfo.InvariantCulture, binder.Type);
                return true;
            }

            public override bool TryBinaryOperation(JValue instance, BinaryOperationBinder binder, object arg, out object result)
            {
                object compareValue = (arg is JValue) ? ((JValue)arg).Value : arg;

                switch (binder.Operation)
                {
                    case ExpressionType.Equal:
                        result = (Compare(instance.Type, instance.Value, compareValue) == 0);
                        return true;
                    case ExpressionType.NotEqual:
                        result = (Compare(instance.Type, instance.Value, compareValue) != 0);
                        return true;
                    case ExpressionType.GreaterThan:
                        result = (Compare(instance.Type, instance.Value, compareValue) > 0);
                        return true;
                    case ExpressionType.GreaterThanOrEqual:
                        result = (Compare(instance.Type, instance.Value, compareValue) >= 0);
                        return true;
                    case ExpressionType.LessThan:
                        result = (Compare(instance.Type, instance.Value, compareValue) < 0);
                        return true;
                    case ExpressionType.LessThanOrEqual:
                        result = (Compare(instance.Type, instance.Value, compareValue) <= 0);
                        return true;
                    case ExpressionType.Add:
                    case ExpressionType.AddAssign:
                    case ExpressionType.Subtract:
                    case ExpressionType.SubtractAssign:
                    case ExpressionType.Multiply:
                    case ExpressionType.MultiplyAssign:
                    case ExpressionType.Divide:
                    case ExpressionType.DivideAssign:
                        if (Operation(binder.Operation, instance.Value, compareValue, out result))
                        {
                            result = new JValue(result);
                            return true;
                        }
                        break;
                }

                result = null;
                return false;
            }

            #endregion
        }

        #endregion
    }
}