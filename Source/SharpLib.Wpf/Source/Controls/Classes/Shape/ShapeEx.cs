using System.Windows;
using System.Windows.Controls;

namespace SharpLib.Wpf.Controls
{
    public class ShapeEx : Control
    {
        #region Конструктор

        static ShapeEx()
        {
            // Переопределение стиля по умолчанию
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ShapeEx), new FrameworkPropertyMetadata(typeof(ShapeEx)));

            WidthProperty.OverrideMetadata(typeof(ButtonEx), new FrameworkPropertyMetadata((double)40));
            HeightProperty.OverrideMetadata(typeof(ButtonEx), new FrameworkPropertyMetadata((double)40));
        }

        #endregion
    }
}