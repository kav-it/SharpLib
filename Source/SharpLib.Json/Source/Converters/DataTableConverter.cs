using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;

namespace SharpLib.Json
{
    public class DataTableConverter : JsonConverter
    {
        #region Методы

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            DataTable table = (DataTable)value;
            DefaultContractResolver resolver = serializer.ContractResolver as DefaultContractResolver;

            writer.WriteStartArray();

            foreach (DataRow row in table.Rows)
            {
                writer.WriteStartObject();
                foreach (DataColumn column in row.Table.Columns)
                {
                    if (serializer.NullValueHandling == NullValueHandling.Ignore && (row[column] == null || row[column] == DBNull.Value))
                    {
                        continue;
                    }

                    writer.WritePropertyName((resolver != null) ? resolver.GetResolvedPropertyName(column.ColumnName) : column.ColumnName);
                    serializer.Serialize(writer, row[column]);
                }
                writer.WriteEndObject();
            }

            writer.WriteEndArray();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            DataTable dt = existingValue as DataTable;

            if (dt == null)
            {
                dt = (objectType == typeof(DataTable))
                    ? new DataTable()
                    : (DataTable)Activator.CreateInstance(objectType);
            }

            if (reader.TokenType == JsonToken.PropertyName)
            {
                dt.TableName = (string)reader.Value;

                CheckedRead(reader);
            }

            if (reader.TokenType != JsonToken.StartArray)
            {
                throw JsonSerializationException.Create(reader, "Unexpected JSON token when reading DataTable. Expected StartArray, got {0}.".FormatWith(CultureInfo.InvariantCulture, reader.TokenType));
            }

            CheckedRead(reader);

            while (reader.TokenType != JsonToken.EndArray)
            {
                CreateRow(reader, dt);

                CheckedRead(reader);
            }

            return dt;
        }

        private static void CreateRow(JsonReader reader, DataTable dt)
        {
            DataRow dr = dt.NewRow();
            CheckedRead(reader);

            while (reader.TokenType == JsonToken.PropertyName)
            {
                string columnName = (string)reader.Value;

                CheckedRead(reader);

                DataColumn column = dt.Columns[columnName];
                if (column == null)
                {
                    Type columnType = GetColumnDataType(reader);
                    column = new DataColumn(columnName, columnType);
                    dt.Columns.Add(column);
                }

                if (column.DataType == typeof(DataTable))
                {
                    if (reader.TokenType == JsonToken.StartArray)
                    {
                        CheckedRead(reader);
                    }

                    DataTable nestedDt = new DataTable();

                    while (reader.TokenType != JsonToken.EndArray)
                    {
                        CreateRow(reader, nestedDt);

                        CheckedRead(reader);
                    }

                    dr[columnName] = nestedDt;
                }
                else if (column.DataType.IsArray && column.DataType != typeof(byte[]))
                {
                    if (reader.TokenType == JsonToken.StartArray)
                    {
                        CheckedRead(reader);
                    }

                    List<object> o = new List<object>();

                    while (reader.TokenType != JsonToken.EndArray)
                    {
                        o.Add(reader.Value);
                        CheckedRead(reader);
                    }

                    Array destinationArray = Array.CreateInstance(column.DataType.GetElementType(), o.Count);
                    Array.Copy(o.ToArray(), destinationArray, o.Count);

                    dr[columnName] = destinationArray;
                }
                else
                {
                    dr[columnName] = reader.Value ?? DBNull.Value;
                }

                CheckedRead(reader);
            }

            dr.EndEdit();
            dt.Rows.Add(dr);
        }

        private static Type GetColumnDataType(JsonReader reader)
        {
            JsonToken tokenType = reader.TokenType;

            switch (tokenType)
            {
                case JsonToken.Integer:
                case JsonToken.Boolean:
                case JsonToken.Float:
                case JsonToken.String:
                case JsonToken.Date:
                case JsonToken.Bytes:
                    return reader.ValueType;
                case JsonToken.Null:
                case JsonToken.Undefined:
                    return typeof(string);
                case JsonToken.StartArray:
                    CheckedRead(reader);
                    if (reader.TokenType == JsonToken.StartObject)
                    {
                        return typeof(DataTable);
                    }

                    Type arrayType = GetColumnDataType(reader);
                    return arrayType.MakeArrayType();
                default:
                    throw JsonSerializationException.Create(reader, "Unexpected JSON token when reading DataTable: {0}".FormatWith(CultureInfo.InvariantCulture, tokenType));
            }
        }

        private static void CheckedRead(JsonReader reader)
        {
            if (!reader.Read())
            {
                throw JsonSerializationException.Create(reader, "Unexpected end when reading DataTable.");
            }
        }

        public override bool CanConvert(Type valueType)
        {
            return typeof(DataTable).IsAssignableFrom(valueType);
        }

        #endregion
    }
}