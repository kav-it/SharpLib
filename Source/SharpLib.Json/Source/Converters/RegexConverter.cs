using System;
using System.Text.RegularExpressions;

namespace SharpLib.Json
{
    public class RegexConverter : JsonConverter
    {
        #region Константы

        private const string OptionsName = "Options";

        private const string PatternName = "Pattern";

        #endregion

        #region Методы

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            Regex regex = (Regex)value;

            BsonWriter bsonWriter = writer as BsonWriter;
            if (bsonWriter != null)
            {
                WriteBson(bsonWriter, regex);
            }
            else
            {
                WriteJson(writer, regex, serializer);
            }
        }

        private bool HasFlag(RegexOptions options, RegexOptions flag)
        {
            return ((options & flag) == flag);
        }

        private void WriteBson(BsonWriter writer, Regex regex)
        {
            string options = null;

            if (HasFlag(regex.Options, RegexOptions.IgnoreCase))
            {
                options += "i";
            }

            if (HasFlag(regex.Options, RegexOptions.Multiline))
            {
                options += "m";
            }

            if (HasFlag(regex.Options, RegexOptions.Singleline))
            {
                options += "s";
            }

            options += "u";

            if (HasFlag(regex.Options, RegexOptions.ExplicitCapture))
            {
                options += "x";
            }

            writer.WriteRegex(regex.ToString(), options);
        }

        private void WriteJson(JsonWriter writer, Regex regex, JsonSerializer serializer)
        {
            DefaultContractResolver resolver = serializer.ContractResolver as DefaultContractResolver;

            writer.WriteStartObject();
            writer.WritePropertyName((resolver != null) ? resolver.GetResolvedPropertyName(PatternName) : PatternName);
            writer.WriteValue(regex.ToString());
            writer.WritePropertyName((resolver != null) ? resolver.GetResolvedPropertyName(OptionsName) : OptionsName);
            serializer.Serialize(writer, regex.Options);
            writer.WriteEndObject();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.StartObject)
            {
                return ReadRegexObject(reader, serializer);
            }

            if (reader.TokenType == JsonToken.String)
            {
                return ReadRegexString(reader);
            }

            throw JsonSerializationException.Create(reader, "Unexpected token when reading Regex.");
        }

        private object ReadRegexString(JsonReader reader)
        {
            string regexText = (string)reader.Value;
            int patternOptionDelimiterIndex = regexText.LastIndexOf('/');

            string patternText = regexText.Substring(1, patternOptionDelimiterIndex - 1);
            string optionsText = regexText.Substring(patternOptionDelimiterIndex + 1);

            RegexOptions options = RegexOptions.None;
            foreach (char c in optionsText)
            {
                switch (c)
                {
                    case 'i':
                        options |= RegexOptions.IgnoreCase;
                        break;
                    case 'm':
                        options |= RegexOptions.Multiline;
                        break;
                    case 's':
                        options |= RegexOptions.Singleline;
                        break;
                    case 'x':
                        options |= RegexOptions.ExplicitCapture;
                        break;
                }
            }

            return new Regex(patternText, options);
        }

        private Regex ReadRegexObject(JsonReader reader, JsonSerializer serializer)
        {
            string pattern = null;
            RegexOptions? options = null;

            while (reader.Read())
            {
                switch (reader.TokenType)
                {
                    case JsonToken.PropertyName:
                        string propertyName = reader.Value.ToString();

                        if (!reader.Read())
                        {
                            throw JsonSerializationException.Create(reader, "Unexpected end when reading Regex.");
                        }

                        if (string.Equals(propertyName, PatternName, StringComparison.OrdinalIgnoreCase))
                        {
                            pattern = (string)reader.Value;
                        }
                        else if (string.Equals(propertyName, OptionsName, StringComparison.OrdinalIgnoreCase))
                        {
                            options = serializer.Deserialize<RegexOptions>(reader);
                        }
                        else
                        {
                            reader.Skip();
                        }
                        break;
                    case JsonToken.Comment:
                        break;
                    case JsonToken.EndObject:
                        if (pattern == null)
                        {
                            throw JsonSerializationException.Create(reader, "Error deserializing Regex. No pattern found.");
                        }

                        return new Regex(pattern, options ?? RegexOptions.None);
                }
            }

            throw JsonSerializationException.Create(reader, "Unexpected end when reading Regex.");
        }

        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(Regex));
        }

        #endregion
    }
}