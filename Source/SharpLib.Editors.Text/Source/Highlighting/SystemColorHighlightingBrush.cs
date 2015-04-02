using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Media;

using SharpLib.Notepad.Rendering;

namespace SharpLib.Notepad.Highlighting
{
    [Serializable]
    internal sealed class SystemColorHighlightingBrush : HighlightingBrush, ISerializable
    {
        #region Поля

        private readonly PropertyInfo property;

        #endregion

        #region Конструктор

        public SystemColorHighlightingBrush(PropertyInfo property)
        {
            Debug.Assert(property.ReflectedType == typeof(SystemColors));
            Debug.Assert(typeof(Brush).IsAssignableFrom(property.PropertyType));
            this.property = property;
        }

        private SystemColorHighlightingBrush(SerializationInfo info, StreamingContext context)
        {
            property = typeof(SystemColors).GetProperty(info.GetString("propertyName"));
            if (property == null)
            {
                throw new ArgumentException("Error deserializing SystemColorHighlightingBrush");
            }
        }

        #endregion

        #region Методы

        public override Brush GetBrush(ITextRunConstructionContext context)
        {
            return (Brush)property.GetValue(null, null);
        }

        public override string ToString()
        {
            return property.Name;
        }

        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("propertyName", property.Name);
        }

        public override bool Equals(object obj)
        {
            var other = obj as SystemColorHighlightingBrush;
            if (other == null)
            {
                return false;
            }
            return object.Equals(property, other.property);
        }

        public override int GetHashCode()
        {
            return property.GetHashCode();
        }

        #endregion
    }
}