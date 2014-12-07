using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;

namespace SharpLib.Json
{
    public class JsonPropertyCollection : KeyedCollection<string, JsonProperty>
    {
        #region Поля

        private readonly Type _type;

        #endregion

        #region Конструктор

        public JsonPropertyCollection(Type type)
            : base(StringComparer.Ordinal)
        {
            ValidationUtils.ArgumentNotNull(type, "type");
            _type = type;
        }

        #endregion

        #region Методы

        protected override string GetKeyForItem(JsonProperty item)
        {
            return item.PropertyName;
        }

        public void AddProperty(JsonProperty property)
        {
            if (Contains(property.PropertyName))
            {
                if (property.Ignored)
                {
                    return;
                }

                JsonProperty existingProperty = this[property.PropertyName];
                bool duplicateProperty = true;

                if (existingProperty.Ignored)
                {
                    Remove(existingProperty);
                    duplicateProperty = false;
                }
                else
                {
                    if (property.DeclaringType != null && existingProperty.DeclaringType != null)
                    {
                        if (property.DeclaringType.IsSubclassOf(existingProperty.DeclaringType))
                        {
                            Remove(existingProperty);
                            duplicateProperty = false;
                        }
                        if (existingProperty.DeclaringType.IsSubclassOf(property.DeclaringType))
                        {
                            return;
                        }
                    }
                }

                if (duplicateProperty)
                {
                    throw new JsonSerializationException(
                        "A member with the name '{0}' already exists on '{1}'. Use the JsonPropertyAttribute to specify another name.".FormatWith(CultureInfo.InvariantCulture, property.PropertyName,
                            _type));
                }
            }

            Add(property);
        }

        public JsonProperty GetClosestMatchProperty(string propertyName)
        {
            var property = GetProperty(propertyName, StringComparison.Ordinal) ?? GetProperty(propertyName, StringComparison.OrdinalIgnoreCase);

            return property;
        }

        private bool TryGetValue(string key, out JsonProperty item)
        {
            if (Dictionary == null)
            {
                item = default(JsonProperty);
                return false;
            }

            return Dictionary.TryGetValue(key, out item);
        }

        public JsonProperty GetProperty(string propertyName, StringComparison comparisonType)
        {
            if (comparisonType == StringComparison.Ordinal)
            {
                JsonProperty property;
                if (TryGetValue(propertyName, out property))
                {
                    return property;
                }

                return null;
            }

            return this
                .FirstOrDefault(property => string.Equals(propertyName, property.PropertyName, comparisonType));
        }

        #endregion
    }
}