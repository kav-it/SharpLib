using System;
using System.Globalization;
using System.Runtime.Serialization;
using System.Windows.Media;

using SharpLib.Texter.Rendering;

namespace SharpLib.Texter.Highlighting
{
    [Serializable]
    public sealed class SimpleHighlightingBrush : HighlightingBrush, ISerializable
    {
        #region Поля

        private readonly SolidColorBrush brush;

        #endregion

        #region Конструктор

        internal SimpleHighlightingBrush(SolidColorBrush brush)
        {
            brush.Freeze();
            this.brush = brush;
        }

        public SimpleHighlightingBrush(Color color) : this(new SolidColorBrush(color))
        {
        }

        private SimpleHighlightingBrush(SerializationInfo info, StreamingContext context)
        {
            brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(info.GetString("color")));
            brush.Freeze();
        }

        #endregion

        #region Методы

        public override Brush GetBrush(ITextRunConstructionContext context)
        {
            return brush;
        }

        public override string ToString()
        {
            return brush.ToString();
        }

        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("color", brush.Color.ToString(CultureInfo.InvariantCulture));
        }

        public override bool Equals(object obj)
        {
            var other = obj as SimpleHighlightingBrush;
            if (other == null)
            {
                return false;
            }
            return brush.Color.Equals(other.brush.Color);
        }

        public override int GetHashCode()
        {
            return brush.Color.GetHashCode();
        }

        #endregion
    }
}