using System;
using System.ComponentModel;
using System.Globalization;

namespace SharpLib.Notepad.Document
{
    public class TextLocationConverter : TypeConverter
    {
        #region Ìועמהû

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(TextLocation) || base.CanConvertTo(context, destinationType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string)
            {
                var parts = ((string)value).Split(';', ',');
                if (parts.Length == 2)
                {
                    return new TextLocation(int.Parse(parts[0], culture), int.Parse(parts[1], culture));
                }
            }
            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (value is TextLocation && destinationType == typeof(string))
            {
                var loc = (TextLocation)value;
                return loc.Line.ToString(culture) + ";" + loc.Column.ToString(culture);
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }

        #endregion
    }
}