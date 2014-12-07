using System;
using System.Data;

namespace SharpLib.Json
{
    public class DataSetConverter : JsonConverter
    {
        #region Методы

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            DataSet dataSet = (DataSet)value;
            DefaultContractResolver resolver = serializer.ContractResolver as DefaultContractResolver;

            DataTableConverter converter = new DataTableConverter();

            writer.WriteStartObject();

            foreach (DataTable table in dataSet.Tables)
            {
                writer.WritePropertyName((resolver != null) ? resolver.GetResolvedPropertyName(table.TableName) : table.TableName);

                converter.WriteJson(writer, table, serializer);
            }

            writer.WriteEndObject();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            DataSet ds = (objectType == typeof(DataSet))
                ? new DataSet()
                : (DataSet)Activator.CreateInstance(objectType);

            DataTableConverter converter = new DataTableConverter();

            CheckedRead(reader);

            while (reader.TokenType == JsonToken.PropertyName)
            {
                DataTable dt = ds.Tables[(string)reader.Value];
                bool exists = (dt != null);

                dt = (DataTable)converter.ReadJson(reader, typeof(DataTable), dt, serializer);

                if (!exists)
                {
                    ds.Tables.Add(dt);
                }

                CheckedRead(reader);
            }

            return ds;
        }

        public override bool CanConvert(Type valueType)
        {
            return typeof(DataSet).IsAssignableFrom(valueType);
        }

        private void CheckedRead(JsonReader reader)
        {
            if (!reader.Read())
            {
                throw JsonSerializationException.Create(reader, "Unexpected end when reading DataSet.");
            }
        }

        #endregion
    }
}