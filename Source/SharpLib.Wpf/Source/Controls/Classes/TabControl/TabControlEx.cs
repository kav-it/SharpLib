using System.Windows;
using System.Windows.Controls;

namespace SharpLib.Wpf.Controls
{
    public class TabControlEx : TabControl
    {
        #region Конструктор

        static TabControlEx()
        {
            // Переопределение стиля по умолчанию
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TabControlEx), new FrameworkPropertyMetadata(typeof(TabControlEx)));
        }

        #endregion
    }
}