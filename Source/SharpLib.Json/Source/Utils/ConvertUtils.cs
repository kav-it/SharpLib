﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlTypes;
using System.Globalization;
using System.Numerics;
using System.Reflection;

namespace SharpLib.Json
{
    internal enum PrimitiveTypeCode
    {
        Empty,

        Object,

        Char,

        CharNullable,

        Boolean,

        BooleanNullable,

        SByte,

        SByteNullable,

        Int16,

        Int16Nullable,

        UInt16,

        UInt16Nullable,

        Int32,

        Int32Nullable,

        Byte,

        ByteNullable,

        UInt32,

        UInt32Nullable,

        Int64,

        Int64Nullable,

        UInt64,

        UInt64Nullable,

        Single,

        SingleNullable,

        Double,

        DoubleNullable,

        DateTime,

        DateTimeNullable,

        DateTimeOffset,

        DateTimeOffsetNullable,

        Decimal,

        DecimalNullable,

        Guid,

        GuidNullable,

        TimeSpan,

        TimeSpanNullable,

        BigInteger,

        BigIntegerNullable,

        Uri,

        String,

        Bytes,

        DBNull
    }

    internal class TypeInformation
    {
        #region Свойства

        public Type Type { get; set; }

        public PrimitiveTypeCode TypeCode { get; set; }

        #endregion
    }

    internal enum ParseResult
    {
        None,

        Success,

        Overflow,

        Invalid
    }

    internal static class ConvertUtils
    {
        #region Перечисления

        internal enum ConvertResult
        {
            Success,

            CannotConvertNull,

            NotInstantiableType,

            NoValidConversion
        }

        #endregion

        #region Поля

        private static readonly ThreadSafeStore<TypeConvertKey, Func<object, object>> CastConverters =
            new ThreadSafeStore<TypeConvertKey, Func<object, object>>(CreateCastConverter);

        private static readonly TypeInformation[] PrimitiveTypeCodes =
        {
            new TypeInformation
            {
                Type = typeof(object),
                TypeCode = PrimitiveTypeCode.Empty
            },
            new TypeInformation
            {
                Type = typeof(object),
                TypeCode = PrimitiveTypeCode.Object
            },
            new TypeInformation
            {
                Type = typeof(object),
                TypeCode = PrimitiveTypeCode.DBNull
            },
            new TypeInformation
            {
                Type = typeof(bool),
                TypeCode = PrimitiveTypeCode.Boolean
            },
            new TypeInformation
            {
                Type = typeof(char),
                TypeCode = PrimitiveTypeCode.Char
            },
            new TypeInformation
            {
                Type = typeof(sbyte),
                TypeCode = PrimitiveTypeCode.SByte
            },
            new TypeInformation
            {
                Type = typeof(byte),
                TypeCode = PrimitiveTypeCode.Byte
            },
            new TypeInformation
            {
                Type = typeof(short),
                TypeCode = PrimitiveTypeCode.Int16
            },
            new TypeInformation
            {
                Type = typeof(ushort),
                TypeCode = PrimitiveTypeCode.UInt16
            },
            new TypeInformation
            {
                Type = typeof(int),
                TypeCode = PrimitiveTypeCode.Int32
            },
            new TypeInformation
            {
                Type = typeof(uint),
                TypeCode = PrimitiveTypeCode.UInt32
            },
            new TypeInformation
            {
                Type = typeof(long),
                TypeCode = PrimitiveTypeCode.Int64
            },
            new TypeInformation
            {
                Type = typeof(ulong),
                TypeCode = PrimitiveTypeCode.UInt64
            },
            new TypeInformation
            {
                Type = typeof(float),
                TypeCode = PrimitiveTypeCode.Single
            },
            new TypeInformation
            {
                Type = typeof(double),
                TypeCode = PrimitiveTypeCode.Double
            },
            new TypeInformation
            {
                Type = typeof(decimal),
                TypeCode = PrimitiveTypeCode.Decimal
            },
            new TypeInformation
            {
                Type = typeof(DateTime),
                TypeCode = PrimitiveTypeCode.DateTime
            },
            new TypeInformation
            {
                Type = typeof(object),
                TypeCode = PrimitiveTypeCode.Empty
            },
            new TypeInformation
            {
                Type = typeof(string),
                TypeCode = PrimitiveTypeCode.String
            }
        };

        private static readonly Dictionary<Type, PrimitiveTypeCode> TypeCodeMap =
            new Dictionary<Type, PrimitiveTypeCode>
            {
                { typeof(char), PrimitiveTypeCode.Char },
                { typeof(char?), PrimitiveTypeCode.CharNullable },
                { typeof(bool), PrimitiveTypeCode.Boolean },
                { typeof(bool?), PrimitiveTypeCode.BooleanNullable },
                { typeof(sbyte), PrimitiveTypeCode.SByte },
                { typeof(sbyte?), PrimitiveTypeCode.SByteNullable },
                { typeof(short), PrimitiveTypeCode.Int16 },
                { typeof(short?), PrimitiveTypeCode.Int16Nullable },
                { typeof(ushort), PrimitiveTypeCode.UInt16 },
                { typeof(ushort?), PrimitiveTypeCode.UInt16Nullable },
                { typeof(int), PrimitiveTypeCode.Int32 },
                { typeof(int?), PrimitiveTypeCode.Int32Nullable },
                { typeof(byte), PrimitiveTypeCode.Byte },
                { typeof(byte?), PrimitiveTypeCode.ByteNullable },
                { typeof(uint), PrimitiveTypeCode.UInt32 },
                { typeof(uint?), PrimitiveTypeCode.UInt32Nullable },
                { typeof(long), PrimitiveTypeCode.Int64 },
                { typeof(long?), PrimitiveTypeCode.Int64Nullable },
                { typeof(ulong), PrimitiveTypeCode.UInt64 },
                { typeof(ulong?), PrimitiveTypeCode.UInt64Nullable },
                { typeof(float), PrimitiveTypeCode.Single },
                { typeof(float?), PrimitiveTypeCode.SingleNullable },
                { typeof(double), PrimitiveTypeCode.Double },
                { typeof(double?), PrimitiveTypeCode.DoubleNullable },
                { typeof(DateTime), PrimitiveTypeCode.DateTime },
                { typeof(DateTime?), PrimitiveTypeCode.DateTimeNullable },
                { typeof(DateTimeOffset), PrimitiveTypeCode.DateTimeOffset },
                { typeof(DateTimeOffset?), PrimitiveTypeCode.DateTimeOffsetNullable },
                { typeof(decimal), PrimitiveTypeCode.Decimal },
                { typeof(decimal?), PrimitiveTypeCode.DecimalNullable },
                { typeof(Guid), PrimitiveTypeCode.Guid },
                { typeof(Guid?), PrimitiveTypeCode.GuidNullable },
                { typeof(TimeSpan), PrimitiveTypeCode.TimeSpan },
                { typeof(TimeSpan?), PrimitiveTypeCode.TimeSpanNullable },
                { typeof(BigInteger), PrimitiveTypeCode.BigInteger },
                { typeof(BigInteger?), PrimitiveTypeCode.BigIntegerNullable },
                { typeof(Uri), PrimitiveTypeCode.Uri },
                { typeof(string), PrimitiveTypeCode.String },
                { typeof(byte[]), PrimitiveTypeCode.Bytes },
                { typeof(DBNull), PrimitiveTypeCode.DBNull }
            };

        #endregion

        #region Методы

        public static PrimitiveTypeCode GetTypeCode(Type t)
        {
            bool isEnum;
            return GetTypeCode(t, out isEnum);
        }

        public static PrimitiveTypeCode GetTypeCode(Type t, out bool isEnum)
        {
            PrimitiveTypeCode typeCode;
            if (TypeCodeMap.TryGetValue(t, out typeCode))
            {
                isEnum = false;
                return typeCode;
            }

            if (t.IsEnum())
            {
                isEnum = true;
                return GetTypeCode(Enum.GetUnderlyingType(t));
            }

            if (ReflectionUtils.IsNullableType(t))
            {
                Type nonNullable = Nullable.GetUnderlyingType(t);
                if (nonNullable.IsEnum())
                {
                    Type nullableUnderlyingType = typeof(Nullable<>).MakeGenericType(Enum.GetUnderlyingType(nonNullable));
                    isEnum = true;
                    return GetTypeCode(nullableUnderlyingType);
                }
            }

            isEnum = false;
            return PrimitiveTypeCode.Object;
        }

        public static TypeInformation GetTypeInformation(IConvertible convertable)
        {
            TypeInformation typeInformation = PrimitiveTypeCodes[(int)convertable.GetTypeCode()];
            return typeInformation;
        }

        public static bool IsConvertible(Type t)
        {
            return typeof(IConvertible).IsAssignableFrom(t);
        }

        public static TimeSpan ParseTimeSpan(string input)
        {
            return TimeSpan.Parse(input, CultureInfo.InvariantCulture);
        }

        private static Func<object, object> CreateCastConverter(TypeConvertKey t)
        {
            MethodInfo castMethodInfo = t.TargetType.GetMethod("op_Implicit", new[] { t.InitialType });
            if (castMethodInfo == null)
            {
                castMethodInfo = t.TargetType.GetMethod("op_Explicit", new[] { t.InitialType });
            }

            if (castMethodInfo == null)
            {
                return null;
            }

            MethodCall<object, object> call = JsonTypeReflector.ReflectionDelegateFactory.CreateMethodCall<object>(castMethodInfo);

            return o => call(null, o);
        }

        internal static BigInteger ToBigInteger(object value)
        {
            if (value is BigInteger)
            {
                return (BigInteger)value;
            }
            if (value is string)
            {
                return BigInteger.Parse((string)value, CultureInfo.InvariantCulture);
            }
            if (value is float)
            {
                return new BigInteger((float)value);
            }
            if (value is double)
            {
                return new BigInteger((double)value);
            }
            if (value is decimal)
            {
                return new BigInteger((decimal)value);
            }
            if (value is int)
            {
                return new BigInteger((int)value);
            }
            if (value is long)
            {
                return new BigInteger((long)value);
            }
            if (value is uint)
            {
                return new BigInteger((uint)value);
            }
            if (value is ulong)
            {
                return new BigInteger((ulong)value);
            }
            if (value is byte[])
            {
                return new BigInteger((byte[])value);
            }

            throw new InvalidCastException("Cannot convert {0} to BigInteger.".FormatWith(CultureInfo.InvariantCulture, value.GetType()));
        }

        public static object FromBigInteger(BigInteger i, Type targetType)
        {
            if (targetType == typeof(decimal))
            {
                return (decimal)i;
            }
            if (targetType == typeof(double))
            {
                return (double)i;
            }
            if (targetType == typeof(float))
            {
                return (float)i;
            }
            if (targetType == typeof(ulong))
            {
                return (ulong)i;
            }

            try
            {
                return System.Convert.ChangeType((long)i, targetType, CultureInfo.InvariantCulture);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Can not convert from BigInteger to {0}.".FormatWith(CultureInfo.InvariantCulture, targetType), ex);
            }
        }

        public static object Convert(object initialValue, CultureInfo culture, Type targetType)
        {
            object value;
            switch (TryConvertInternal(initialValue, culture, targetType, out value))
            {
                case ConvertResult.Success:
                    return value;
                case ConvertResult.CannotConvertNull:
                    throw new Exception("Can not convert null {0} into non-nullable {1}.".FormatWith(CultureInfo.InvariantCulture, initialValue.GetType(), targetType));
                case ConvertResult.NotInstantiableType:
                    throw new ArgumentException("Target type {0} is not a value type or a non-abstract class.".FormatWith(CultureInfo.InvariantCulture, targetType), "targetType");
                case ConvertResult.NoValidConversion:
                    throw new InvalidOperationException("Can not convert from {0} to {1}.".FormatWith(CultureInfo.InvariantCulture, initialValue.GetType(), targetType));
                default:
                    throw new InvalidOperationException("Unexpected conversion result.");
            }
        }

        private static bool TryConvert(object initialValue, CultureInfo culture, Type targetType, out object value)
        {
            try
            {
                if (TryConvertInternal(initialValue, culture, targetType, out value) == ConvertResult.Success)
                {
                    return true;
                }

                value = null;
                return false;
            }
            catch
            {
                value = null;
                return false;
            }
        }

        private static ConvertResult TryConvertInternal(object initialValue, CultureInfo culture, Type targetType, out object value)
        {
            if (initialValue == null)
            {
                throw new ArgumentNullException("initialValue");
            }

            if (ReflectionUtils.IsNullableType(targetType))
            {
                targetType = Nullable.GetUnderlyingType(targetType);
            }

            Type initialType = initialValue.GetType();

            if (targetType == initialType)
            {
                value = initialValue;
                return ConvertResult.Success;
            }

            if (ConvertUtils.IsConvertible(initialValue.GetType()) && ConvertUtils.IsConvertible(targetType))
            {
                if (targetType.IsEnum())
                {
                    if (initialValue is string)
                    {
                        value = Enum.Parse(targetType, initialValue.ToString(), true);
                        return ConvertResult.Success;
                    }
                    if (IsInteger(initialValue))
                    {
                        value = Enum.ToObject(targetType, initialValue);
                        return ConvertResult.Success;
                    }
                }

                value = System.Convert.ChangeType(initialValue, targetType, culture);
                return ConvertResult.Success;
            }

#if !NET20
            if (initialValue is DateTime && targetType == typeof(DateTimeOffset))
            {
                value = new DateTimeOffset((DateTime)initialValue);
                return ConvertResult.Success;
            }
#endif

            if (initialValue is byte[] && targetType == typeof(Guid))
            {
                value = new Guid((byte[])initialValue);
                return ConvertResult.Success;
            }

            if (initialValue is Guid && targetType == typeof(byte[]))
            {
                value = ((Guid)initialValue).ToByteArray();
                return ConvertResult.Success;
            }

            if (initialValue is string)
            {
                if (targetType == typeof(Guid))
                {
                    value = new Guid((string)initialValue);
                    return ConvertResult.Success;
                }
                if (targetType == typeof(Uri))
                {
                    value = new Uri((string)initialValue, UriKind.RelativeOrAbsolute);
                    return ConvertResult.Success;
                }
                if (targetType == typeof(TimeSpan))
                {
                    value = ParseTimeSpan((string)initialValue);
                    return ConvertResult.Success;
                }
                if (targetType == typeof(byte[]))
                {
                    value = System.Convert.FromBase64String((string)initialValue);
                    return ConvertResult.Success;
                }
                if (typeof(Type).IsAssignableFrom(targetType))
                {
                    value = Type.GetType((string)initialValue, true);
                    return ConvertResult.Success;
                }
            }

#if !(NET20 || NET35 || PORTABLE40 || PORTABLE)
            if (targetType == typeof(BigInteger))
            {
                value = ToBigInteger(initialValue);
                return ConvertResult.Success;
            }
            if (initialValue is BigInteger)
            {
                value = FromBigInteger((BigInteger)initialValue, targetType);
                return ConvertResult.Success;
            }
#endif

#if !(NETFX_CORE || PORTABLE40 || PORTABLE)

            TypeConverter toConverter = GetConverter(initialType);

            if (toConverter != null && toConverter.CanConvertTo(targetType))
            {
                value = toConverter.ConvertTo(null, culture, initialValue, targetType);
                return ConvertResult.Success;
            }

            TypeConverter fromConverter = GetConverter(targetType);

            if (fromConverter != null && fromConverter.CanConvertFrom(initialType))
            {
                value = fromConverter.ConvertFrom(null, culture, initialValue);
                return ConvertResult.Success;
            }
#endif
#if !(NETFX_CORE || PORTABLE40 || PORTABLE)

            if (initialValue == DBNull.Value)
            {
                if (ReflectionUtils.IsNullable(targetType))
                {
                    value = EnsureTypeAssignable(null, initialType, targetType);
                    return ConvertResult.Success;
                }

                value = null;
                return ConvertResult.CannotConvertNull;
            }
#endif
#if !(NETFX_CORE || PORTABLE40 || PORTABLE)
            if (initialValue is INullable)
            {
                value = EnsureTypeAssignable(ToValue((INullable)initialValue), initialType, targetType);
                return ConvertResult.Success;
            }
#endif

            if (targetType.IsInterface() || targetType.IsGenericTypeDefinition() || targetType.IsAbstract())
            {
                value = null;
                return ConvertResult.NotInstantiableType;
            }

            value = null;
            return ConvertResult.NoValidConversion;
        }

        public static object ConvertOrCast(object initialValue, CultureInfo culture, Type targetType)
        {
            object convertedValue;

            if (targetType == typeof(object))
            {
                return initialValue;
            }

            if (initialValue == null && ReflectionUtils.IsNullable(targetType))
            {
                return null;
            }

            if (TryConvert(initialValue, culture, targetType, out convertedValue))
            {
                return convertedValue;
            }

            return EnsureTypeAssignable(initialValue, ReflectionUtils.GetObjectType(initialValue), targetType);
        }

        private static object EnsureTypeAssignable(object value, Type initialType, Type targetType)
        {
            Type valueType = (value != null) ? value.GetType() : null;

            if (value != null)
            {
                if (targetType.IsAssignableFrom(valueType))
                {
                    return value;
                }

                Func<object, object> castConverter = CastConverters.Get(new TypeConvertKey(valueType, targetType));
                if (castConverter != null)
                {
                    return castConverter(value);
                }
            }
            else
            {
                if (ReflectionUtils.IsNullable(targetType))
                {
                    return null;
                }
            }

            throw new ArgumentException("Could not cast or convert from {0} to {1}.".FormatWith(CultureInfo.InvariantCulture, (initialType != null) ? initialType.ToString() : "{null}", targetType));
        }

        public static object ToValue(INullable nullableValue)
        {
            if (nullableValue == null)
            {
                return null;
            }
            if (nullableValue is SqlInt32)
            {
                return ToValue((SqlInt32)nullableValue);
            }
            if (nullableValue is SqlInt64)
            {
                return ToValue((SqlInt64)nullableValue);
            }
            if (nullableValue is SqlBoolean)
            {
                return ToValue((SqlBoolean)nullableValue);
            }
            if (nullableValue is SqlString)
            {
                return ToValue((SqlString)nullableValue);
            }
            if (nullableValue is SqlDateTime)
            {
                return ToValue((SqlDateTime)nullableValue);
            }

            throw new ArgumentException("Unsupported INullable type: {0}".FormatWith(CultureInfo.InvariantCulture, nullableValue.GetType()));
        }

        internal static TypeConverter GetConverter(Type t)
        {
            return JsonTypeReflector.GetTypeConverter(t);
        }

        public static bool IsInteger(object value)
        {
            switch (GetTypeCode(value.GetType()))
            {
                case PrimitiveTypeCode.SByte:
                case PrimitiveTypeCode.Byte:
                case PrimitiveTypeCode.Int16:
                case PrimitiveTypeCode.UInt16:
                case PrimitiveTypeCode.Int32:
                case PrimitiveTypeCode.UInt32:
                case PrimitiveTypeCode.Int64:
                case PrimitiveTypeCode.UInt64:
                    return true;
                default:
                    return false;
            }
        }

        public static ParseResult Int32TryParse(char[] chars, int start, int length, out int value)
        {
            value = 0;

            if (length == 0)
            {
                return ParseResult.Invalid;
            }

            bool isNegative = (chars[start] == '-');

            if (isNegative)
            {
                if (length == 1)
                {
                    return ParseResult.Invalid;
                }

                start++;
                length--;
            }

            int end = start + length;

            for (int i = start; i < end; i++)
            {
                int c = chars[i] - '0';

                if (c < 0 || c > 9)
                {
                    return ParseResult.Invalid;
                }

                int newValue = (10 * value) - c;

                if (newValue > value)
                {
                    i++;

                    for (; i < end; i++)
                    {
                        c = chars[i] - '0';

                        if (c < 0 || c > 9)
                        {
                            return ParseResult.Invalid;
                        }
                    }

                    return ParseResult.Overflow;
                }

                value = newValue;
            }

            if (!isNegative)
            {
                if (value == int.MinValue)
                {
                    return ParseResult.Overflow;
                }

                value = -value;
            }

            return ParseResult.Success;
        }

        public static ParseResult Int64TryParse(char[] chars, int start, int length, out long value)
        {
            value = 0;

            if (length == 0)
            {
                return ParseResult.Invalid;
            }

            bool isNegative = (chars[start] == '-');

            if (isNegative)
            {
                if (length == 1)
                {
                    return ParseResult.Invalid;
                }

                start++;
                length--;
            }

            int end = start + length;

            for (int i = start; i < end; i++)
            {
                int c = chars[i] - '0';

                if (c < 0 || c > 9)
                {
                    return ParseResult.Invalid;
                }

                long newValue = (10 * value) - c;

                if (newValue > value)
                {
                    i++;

                    for (; i < end; i++)
                    {
                        c = chars[i] - '0';

                        if (c < 0 || c > 9)
                        {
                            return ParseResult.Invalid;
                        }
                    }

                    return ParseResult.Overflow;
                }

                value = newValue;
            }

            if (!isNegative)
            {
                if (value == long.MinValue)
                {
                    return ParseResult.Overflow;
                }

                value = -value;
            }

            return ParseResult.Success;
        }

        public static bool TryConvertGuid(string s, out Guid g)
        {
            return Guid.TryParse(s, out g);
        }

        #endregion

        #region Вложенный класс: TypeConvertKey

        internal struct TypeConvertKey : IEquatable<TypeConvertKey>
        {
            #region Поля

            private readonly Type _initialType;

            private readonly Type _targetType;

            #endregion

            #region Свойства

            public Type InitialType
            {
                get { return _initialType; }
            }

            public Type TargetType
            {
                get { return _targetType; }
            }

            #endregion

            #region Конструктор

            public TypeConvertKey(Type initialType, Type targetType)
            {
                _initialType = initialType;
                _targetType = targetType;
            }

            #endregion

            #region Методы

            public override int GetHashCode()
            {
                return _initialType.GetHashCode() ^ _targetType.GetHashCode();
            }

            public override bool Equals(object obj)
            {
                if (!(obj is TypeConvertKey))
                {
                    return false;
                }

                return Equals((TypeConvertKey)obj);
            }

            public bool Equals(TypeConvertKey other)
            {
                return (_initialType == other._initialType && _targetType == other._targetType);
            }

            #endregion
        }

        #endregion
    }
}