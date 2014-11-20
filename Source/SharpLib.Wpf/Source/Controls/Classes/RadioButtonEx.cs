using System.Windows;
using System.Windows.Controls;

namespace SharpLib.Wpf.Controls
{
    public class RadioButtonEx : RadioButton
    {
        #region Конструктор

        static RadioButtonEx()
        {
            // Переопределение стиля по умолчанию
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RadioButtonEx), new FrameworkPropertyMetadata(typeof(RadioButtonEx)));

            // Установка вертикального выравнивания текста по умолчанию
            VerticalContentAlignmentProperty.OverrideMetadata(typeof(RadioButtonEx), new FrameworkPropertyMetadata(VerticalAlignment.Center));
        }

        #endregion
    }
}