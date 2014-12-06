using System.Windows;
using System.Windows.Controls;

namespace SharpLib.Wpf.Controls
{
    public class ComboBoxEx : ComboBox
    {
        #region Конструктор

        static ComboBoxEx()
        {
            // Переопределение стиля по умолчанию
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ComboBoxEx), new FrameworkPropertyMetadata(typeof(ComboBoxEx)));
        }

        #endregion
    }
}