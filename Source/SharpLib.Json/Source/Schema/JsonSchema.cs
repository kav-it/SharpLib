using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

using SharpLib.Json.Linq;

namespace SharpLib.Json.Schema
{
    public class JsonSchema
    {
        #region Поля

        private readonly string _internalId = Guid.NewGuid().ToString("N");

        #endregion

        #region Свойства

        public string Id { get; set; }

        public string Title { get; set; }

        public bool? Required { get; set; }

        public bool? ReadOnly { get; set; }

        public bool? Hidden { get; set; }

        public bool? Transient { get; set; }

        public string Description { get; set; }

        public JsonSchemaType? Type { get; set; }

        public string Pattern { get; set; }

        public int? MinimumLength { get; set; }

        public int? MaximumLength { get; set; }

        public double? DivisibleBy { get; set; }

        public double? Minimum { get; set; }

        public double? Maximum { get; set; }

        public bool? ExclusiveMinimum { get; set; }

        public bool? ExclusiveMaximum { get; set; }

        public int? MinimumItems { get; set; }

        public int? MaximumItems { get; set; }

        public IList<JsonSchema> Items { get; set; }

        public bool PositionalItemsValidation { get; set; }

        public JsonSchema AdditionalItems { get; set; }

        public bool AllowAdditionalItems { get; set; }

        public bool UniqueItems { get; set; }

        public IDictionary<string, JsonSchema> Properties { get; set; }

        public JsonSchema AdditionalProperties { get; set; }

        public IDictionary<string, JsonSchema> PatternProperties { get; set; }

        public bool AllowAdditionalProperties { get; set; }

        public string Requires { get; set; }

        public IList<JToken> Enum { get; set; }

        public JsonSchemaType? Disallow { get; set; }

        public JToken Default { get; set; }

        public IList<JsonSchema> Extends { get; set; }

        public string Format { get; set; }

        internal string Location { get; set; }

        internal string InternalId
        {
            get { return _internalId; }
        }

        internal string DeferredReference { get; set; }

        internal bool ReferencesResolved { get; set; }

        #endregion

        #region Конструктор

        public JsonSchema()
        {
            AllowAdditionalProperties = true;
            AllowAdditionalItems = true;
        }

        #endregion

        #region Методы

        public static JsonSchema Read(JsonReader reader)
        {
            return Read(reader, new JsonSchemaResolver());
        }

        public static JsonSchema Read(JsonReader reader, JsonSchemaResolver resolver)
        {
            ValidationUtils.ArgumentNotNull(reader, "reader");
            ValidationUtils.ArgumentNotNull(resolver, "resolver");

            JsonSchemaBuilder builder = new JsonSchemaBuilder(resolver);
            return builder.Read(reader);
        }

        public static JsonSchema Parse(string json)
        {
            return Parse(json, new JsonSchemaResolver());
        }

        public static JsonSchema Parse(string json, JsonSchemaResolver resolver)
        {
            ValidationUtils.ArgumentNotNull(json, "json");

            using (JsonReader reader = new JsonTextReader(new StringReader(json)))
            {
                return Read(reader, resolver);
            }
        }

        public void WriteTo(JsonWriter writer)
        {
            WriteTo(writer, new JsonSchemaResolver());
        }

        public void WriteTo(JsonWriter writer, JsonSchemaResolver resolver)
        {
            ValidationUtils.ArgumentNotNull(writer, "writer");
            ValidationUtils.ArgumentNotNull(resolver, "resolver");

            JsonSchemaWriter schemaWriter = new JsonSchemaWriter(writer, resolver);
            schemaWriter.WriteSchema(this);
        }

        public override string ToString()
        {
            StringWriter writer = new StringWriter(CultureInfo.InvariantCulture);
            JsonTextWriter jsonWriter = new JsonTextWriter(writer);
            jsonWriter.Formatting = JsonFormatting.Indented;

            WriteTo(jsonWriter);

            return writer.ToString();
        }

        #endregion
    }
}