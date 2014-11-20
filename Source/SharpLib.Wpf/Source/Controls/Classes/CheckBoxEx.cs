using System.Windows;
using System.Windows.Controls;

namespace SharpLib.Wpf.Controls
{
    public class CheckBoxEx : CheckBox
    {
        #region Конструктор

        static CheckBoxEx()
        {
            // Переопределение стиля по умолчанию
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CheckBoxEx), new FrameworkPropertyMetadata(typeof(CheckBoxEx)));

            // Установка вертикального выравнивания текста по умолчанию
            VerticalContentAlignmentProperty.OverrideMetadata(typeof(CheckBoxEx), new FrameworkPropertyMetadata(VerticalAlignment.Center));
        }

        #endregion
    }
}