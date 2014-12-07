using System;
using System.Globalization;
using System.Reflection;
using System.Runtime.Serialization;
using System.Linq;

namespace SharpLib.Json
{
    public class StringEnumConverter : JsonConverter
    {
        #region Поля

        private static readonly ThreadSafeStore<Type, BidirectionalDictionary<string, string>> EnumMemberNamesPerType =
            new ThreadSafeStore<Type, BidirectionalDictionary<string, string>>(InitializeEnumType);

        #endregion

        #region Свойства

        public bool CamelCaseText { get; set; }

        public bool AllowIntegerValues { get; set; }

        #endregion

        #region Конструктор

        public StringEnumConverter()
        {
            AllowIntegerValues = true;
        }

        #endregion

        #region Методы

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            Enum e = (Enum)value;

            string enumName = e.ToString("G");

            if (char.IsNumber(enumName[0]) || enumName[0] == '-')
            {
                writer.WriteValue(value);
            }
            else
            {
                BidirectionalDictionary<string, string> map = EnumMemberNamesPerType.Get(e.GetType());

                string[] names = enumName.Split(',');
                for (int i = 0; i < names.Length; i++)
                {
                    string name = names[i].Trim();

                    string resolvedEnumName;
                    map.TryGetByFirst(name, out resolvedEnumName);
                    resolvedEnumName = resolvedEnumName ?? name;

                    if (CamelCaseText)
                    {
                        resolvedEnumName = StringUtils.ToCamelCase(resolvedEnumName);
                    }

                    names[i] = resolvedEnumName;
                }

                string finalName = string.Join(", ", names);

                writer.WriteValue(finalName);
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            bool isNullable = ReflectionUtils.IsNullableType(objectType);
            Type t = isNullable ? Nullable.GetUnderlyingType(objectType) : objectType;

            if (reader.TokenType == JsonToken.Null)
            {
                if (!ReflectionUtils.IsNullableType(objectType))
                {
                    throw JsonSerializationException.Create(reader, "Cannot convert null value to {0}.".FormatWith(CultureInfo.InvariantCulture, objectType));
                }

                return null;
            }

            try
            {
                if (reader.TokenType == JsonToken.String)
                {
                    string enumText = reader.Value.ToString();
                    if (enumText == string.Empty && isNullable)
                    {
                        return null;
                    }

                    string finalEnumText;

                    BidirectionalDictionary<string, string> map = EnumMemberNamesPerType.Get(t);
                    if (enumText.IndexOf(',') != -1)
                    {
                        string[] names = enumText.Split(',');
                        for (int i = 0; i < names.Length; i++)
                        {
                            string name = names[i].Trim();

                            names[i] = ResolvedEnumName(map, name);
                        }

                        finalEnumText = string.Join(", ", names);
                    }
                    else
                    {
                        finalEnumText = ResolvedEnumName(map, enumText);
                    }

                    return Enum.Parse(t, finalEnumText, true);
                }

                if (reader.TokenType == JsonToken.Integer)
                {
                    if (!AllowIntegerValues)
                    {
                        throw JsonSerializationException.Create(reader, "Integer value {0} is not allowed.".FormatWith(CultureInfo.InvariantCulture, reader.Value));
                    }

                    return ConvertUtils.ConvertOrCast(reader.Value, CultureInfo.InvariantCulture, t);
                }
            }
            catch (Exception ex)
            {
                throw JsonSerializationException.Create(reader,
                    "Error converting value {0} to type '{1}'.".FormatWith(CultureInfo.InvariantCulture, MiscellaneousUtils.FormatValueForPrint(reader.Value), objectType), ex);
            }

            throw JsonSerializationException.Create(reader, "Unexpected token {0} when parsing enum.".FormatWith(CultureInfo.InvariantCulture, reader.TokenType));
        }

        private static string ResolvedEnumName(BidirectionalDictionary<string, string> map, string enumText)
        {
            string resolvedEnumName;
            map.TryGetBySecond(enumText, out resolvedEnumName);
            resolvedEnumName = resolvedEnumName ?? enumText;
            return resolvedEnumName;
        }

        public override bool CanConvert(Type objectType)
        {
            Type t = (ReflectionUtils.IsNullableType(objectType))
                ? Nullable.GetUnderlyingType(objectType)
                : objectType;

            return t.IsEnum();
        }

        private static BidirectionalDictionary<string, string> InitializeEnumType(Type type)
        {
            BidirectionalDictionary<string, string> map = new BidirectionalDictionary<string, string>(
                StringComparer.OrdinalIgnoreCase,
                StringComparer.OrdinalIgnoreCase);

            foreach (FieldInfo f in type.GetFields())
            {
                string n1 = f.Name;
                string n2 = f.GetCustomAttributes(typeof(EnumMemberAttribute), true)
                    .Cast<EnumMemberAttribute>()
                    .Select(a => a.Value)
                    .SingleOrDefault() ?? f.Name;

                string s;
                if (map.TryGetBySecond(n2, out s))
                {
                    throw new InvalidOperationException("Enum name '{0}' already exists on enum '{1}'.".FormatWith(CultureInfo.InvariantCulture, n2, type.Name));
                }

                map.Set(n1, n2);
            }

            return map;
        }

        #endregion
    }
}