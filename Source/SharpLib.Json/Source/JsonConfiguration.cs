using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace SharpLib.Json
{
    public class JsonConfiguration
    {
        private const string KEY_DELIMETR = ":";

        public Dictionary<string, string> Data { get; private set; }

        public string Filename { get; private set; }

        public JsonConfiguration(string filename)
        {
            Filename = filename;
            Data = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        }

        public void Load()
        {
            using (var stream = new FileStream(Filename, FileMode.Open, FileAccess.Read))
            {
                Load(stream);
            }
        }

        private void Load(Stream stream)
        {
            var data = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            using (var reader = new JsonTextReader(new StreamReader(stream)))
            {
                var startObjectCount = 0;

                // Dates are parsed as strings
                reader.DateParseHandling = DateParseHandling.None;

                // Move to the first token
                reader.Read();

                SkipComments(reader);

                if (reader.TokenType != JsonToken.StartObject)
                {
                    throw new FormatException();
                }

                do
                {
                    SkipComments(reader);

                    switch (reader.TokenType)
                    {
                        case JsonToken.StartObject:
                            startObjectCount++;
                            break;

                        case JsonToken.EndObject:
                            startObjectCount--;
                            break;

                        // Keys in key-value pairs
                        case JsonToken.PropertyName:
                            break;

                        // Values in key-value pairs
                        case JsonToken.Integer:
                        case JsonToken.Float:
                        case JsonToken.String:
                        case JsonToken.Boolean:
                        case JsonToken.Bytes:
                        case JsonToken.Raw:
                        case JsonToken.Null:
                            var key = reader.Path.Replace(".", KEY_DELIMETR);
                            if (data.ContainsKey(key))
                            {
                                throw new FormatException("KeyIsDuplicated");
                            }
                            data[key] = reader.Value.ToString();
                            break;

                        // End of file
                        case JsonToken.None:
                            {
                                throw new FormatException("UnexpectedEnd");
                            }

                        default:
                            {
                                throw new FormatException("UnsupportedJSONToken");
                            }
                    }

                    reader.Read();

                } while (startObjectCount > 0);
            }

            ReplaceData(data);
        }

        public void Save()
        {
            // Because we need to read the original contents while generating new contents, the new contents are
            // cached in memory and used to overwrite original contents after we finish reading the original contents
            using (var cacheStream = new MemoryStream())
            {
                using (var inputStream = new FileStream(Filename, FileMode.Open))
                {
                    Commit(inputStream, cacheStream);
                }

                cacheStream.Seek(0, SeekOrigin.Begin);
                using (var outputStream = new FileStream(Filename, FileMode.Truncate))
                {
                    cacheStream.CopyTo(outputStream);
                }
            } 
        }

        public bool TryGet(string key, out string value)
        {
            return Data.TryGetValue(key, out value);
        }

        public void Set(string key, string value)
        {
            Data[key] = value;
        }

        private void ReplaceData(Dictionary<string, string> data)
        {
            Data = data;
        }

        private void SkipComments(JsonReader reader)
        {
            while (reader.TokenType == JsonToken.Comment)
            {
                reader.Read();
            }
        }

        private void CopyComments(JsonReader inputReader, JsonWriter outputStream)
        {
            while (inputReader.TokenType == JsonToken.Comment)
            {
                outputStream.WriteComment(inputReader.Value.ToString());
                inputReader.Read();
            }
        }

        private void Commit(Stream inputStream, Stream outputStream)
        {
            var processedKeys = new HashSet<string>();
            var outputWriter = new JsonTextWriter(new StreamWriter(outputStream));
            outputWriter.Formatting = JsonFormatting.Indented;

            using (var inputReader = new JsonTextReader(new StreamReader(inputStream)))
            {
                var startObjectCount = 0;

                // Dates are parsed as strings
                inputReader.DateParseHandling = DateParseHandling.None;

                // Move to the first token
                inputReader.Read();

                CopyComments(inputReader, outputWriter);

                if (inputReader.TokenType != JsonToken.StartObject)
                {
                    throw new FormatException("RootMustBeAnObject");
                }

                do
                {
                    CopyComments(inputReader, outputWriter);

                    switch (inputReader.TokenType)
                    {
                        case JsonToken.StartObject:
                            outputWriter.WriteStartObject();
                            startObjectCount++;
                            break;

                        case JsonToken.EndObject:
                            outputWriter.WriteEndObject();
                            startObjectCount--;
                            break;

                        // Keys in key-value pairs
                        case JsonToken.PropertyName:
                            outputWriter.WritePropertyName(inputReader.Value.ToString());
                            break;

                        // Values in key-value pairs
                        case JsonToken.Integer:
                        case JsonToken.Float:
                        case JsonToken.String:
                        case JsonToken.Boolean:
                        case JsonToken.Bytes:
                        case JsonToken.Raw:
                        case JsonToken.Null:
                            var key = inputReader.Path.Replace(".", KEY_DELIMETR);
                            if (!Data.ContainsKey(key))
                            {
                                throw new InvalidOperationException("CommitWhenNewKeyFound");
                            }
                            outputWriter.WriteValue(Data[key]);
                            processedKeys.Add(key);
                            break;

                        // End of file
                        case JsonToken.None:
                            {
                                throw new FormatException("UnexpectedEnd");
                            }

                        default:
                            {
                                throw new FormatException("UnsupportedJSONToken");
                            }
                    }

                    inputReader.Read();

                } while (startObjectCount > 0);

                CopyComments(inputReader, outputWriter);
                outputWriter.Flush();
            }

            if (Data.Count != processedKeys.Count)
            {
                var missingKeys = string.Join(", ", Data.Keys.Except(processedKeys));
                throw new InvalidOperationException("CommitWhenKeyMissing");
            }
        }


    }
}
