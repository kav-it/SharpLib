using System;
using System.ComponentModel;
using System.Globalization;

namespace SharpLib.Notepad.Highlighting
{
    public sealed class HighlightingDefinitionTypeConverter : TypeConverter
    {
        #region Методы

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
            {
                return true;
            }
            return base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            string definitionName = value as string;
            if (definitionName != null)
            {
                return HighlightingManager.Instance.GetDefinitionByName(definitionName);
            }
            return base.ConvertFrom(context, culture, value);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                return true;
            }
            return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            var definition = value as IHighlightingDefinition;
            if (definition != null && destinationType == typeof(string))
            {
                return definition.Name;
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }

        #endregion
    }
}