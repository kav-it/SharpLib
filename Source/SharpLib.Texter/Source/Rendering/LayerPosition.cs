using System;
using System.Windows;

namespace SharpLib.Texter.Rendering
{
    internal sealed class LayerPosition : IComparable<LayerPosition>
    {
        #region Поля

        internal static readonly DependencyProperty LayerPositionProperty =
            DependencyProperty.RegisterAttached("LayerPosition", typeof(LayerPosition), typeof(LayerPosition));

        internal readonly KnownLayer KnownLayer;

        internal readonly LayerInsertionPosition Position;

        #endregion

        #region Конструктор

        public LayerPosition(KnownLayer knownLayer, LayerInsertionPosition position)
        {
            KnownLayer = knownLayer;
            Position = position;
        }

        #endregion

        #region Методы

        public static void SetLayerPosition(UIElement layer, LayerPosition value)
        {
            layer.SetValue(LayerPositionProperty, value);
        }

        public static LayerPosition GetLayerPosition(UIElement layer)
        {
            return (LayerPosition)layer.GetValue(LayerPositionProperty);
        }

        public int CompareTo(LayerPosition other)
        {
            int r = KnownLayer.CompareTo(other.KnownLayer);
            if (r != 0)
            {
                return r;
            }
            return Position.CompareTo(other.Position);
        }

        #endregion
    }
}