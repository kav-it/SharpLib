﻿using System.Collections.Generic;

namespace SharpLib.Json.Schema
{
    internal static class JsonSchemaConstants
    {
        #region Константы

        public const string AdditionalItemsPropertyName = "additionalItems";

        public const string AdditionalPropertiesPropertyName = "additionalProperties";

        public const string DefaultPropertyName = "default";

        public const string DescriptionPropertyName = "description";

        public const string DisallowPropertyName = "disallow";

        public const string DivisibleByPropertyName = "divisibleBy";

        public const string EnumPropertyName = "enum";

        public const string ExclusiveMaximumPropertyName = "exclusiveMaximum";

        public const string ExclusiveMinimumPropertyName = "exclusiveMinimum";

        public const string ExtendsPropertyName = "extends";

        public const string FormatPropertyName = "format";

        public const string HiddenPropertyName = "hidden";

        public const string IdPropertyName = "id";

        public const string ItemsPropertyName = "items";

        public const string MaximumItemsPropertyName = "maxItems";

        public const string MaximumLengthPropertyName = "maxLength";

        public const string MaximumPropertyName = "maximum";

        public const string MinimumItemsPropertyName = "minItems";

        public const string MinimumLengthPropertyName = "minLength";

        public const string MinimumPropertyName = "minimum";

        public const string OptionLabelPropertyName = "label";

        public const string OptionValuePropertyName = "value";

        public const string PatternPropertiesPropertyName = "patternProperties";

        public const string PatternPropertyName = "pattern";

        public const string PropertiesPropertyName = "properties";

        public const string ReadOnlyPropertyName = "readonly";

        public const string RequiredPropertyName = "required";

        public const string RequiresPropertyName = "requires";

        public const string TitlePropertyName = "title";

        public const string TransientPropertyName = "transient";

        public const string TypePropertyName = "type";

        public const string UniqueItemsPropertyName = "uniqueItems";

        #endregion

        #region Поля

        public static readonly IDictionary<string, JsonSchemaType> JsonSchemaTypeMapping = new Dictionary<string, JsonSchemaType>
        {
            { "string", JsonSchemaType.String },
            { "object", JsonSchemaType.Object },
            { "integer", JsonSchemaType.Integer },
            { "number", JsonSchemaType.Float },
            { "null", JsonSchemaType.Null },
            { "boolean", JsonSchemaType.Boolean },
            { "array", JsonSchemaType.Array },
            { "any", JsonSchemaType.Any }
        };

        #endregion
    }
}