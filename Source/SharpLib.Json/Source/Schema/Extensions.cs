#region License

#endregion

using System.Collections.Generic;

using SharpLib.Json.Linq;

namespace SharpLib.Json.Schema
{
    public static class Extensions
    {
        #region Методы

        public static bool IsValid(this JToken source, JsonSchema schema)
        {
            bool valid = true;
            source.Validate(schema, (sender, args) => { valid = false; });
            return valid;
        }

        public static bool IsValid(this JToken source, JsonSchema schema, out IList<string> errorMessages)
        {
            IList<string> errors = new List<string>();

            source.Validate(schema, (sender, args) => errors.Add(args.Message));

            errorMessages = errors;
            return (errorMessages.Count == 0);
        }

        public static void Validate(this JToken source, JsonSchema schema)
        {
            source.Validate(schema, null);
        }

        public static void Validate(this JToken source, JsonSchema schema, ValidationEventHandler validationEventHandler)
        {
            ValidationUtils.ArgumentNotNull(source, "source");
            ValidationUtils.ArgumentNotNull(schema, "schema");

            using (JsonValidatingReader reader = new JsonValidatingReader(source.CreateReader()))
            {
                reader.Schema = schema;
                if (validationEventHandler != null)
                {
                    reader.ValidationEventHandler += validationEventHandler;
                }

                while (reader.Read())
                {
                }
            }
        }

        #endregion
    }
}