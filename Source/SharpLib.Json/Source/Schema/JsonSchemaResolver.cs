using System;
using System.Collections.Generic;
using System.Linq;

namespace SharpLib.Json.Schema
{
    public class JsonSchemaResolver
    {
        #region Свойства

        public IList<JsonSchema> LoadedSchemas { get; protected set; }

        #endregion

        #region Конструктор

        public JsonSchemaResolver()
        {
            LoadedSchemas = new List<JsonSchema>();
        }

        #endregion

        #region Методы

        public virtual JsonSchema GetSchema(string reference)
        {
            JsonSchema schema = LoadedSchemas.SingleOrDefault(s => string.Equals(s.Id, reference, StringComparison.Ordinal)) ??
                                LoadedSchemas.SingleOrDefault(s => string.Equals(s.Location, reference, StringComparison.Ordinal));

            return schema;
        }

        #endregion
    }
}