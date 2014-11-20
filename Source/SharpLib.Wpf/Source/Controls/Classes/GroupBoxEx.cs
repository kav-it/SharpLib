using System.Windows;
using System.Windows.Controls;

namespace SharpLib.Wpf.Controls
{
    public class GroupBoxEx : GroupBox
    {
        #region Конструктор

        static GroupBoxEx()
        {
            // Переопределение стиля по умолчанию
            DefaultStyleKeyProperty.OverrideMetadata(typeof(GroupBoxEx), new FrameworkPropertyMetadata(typeof(GroupBoxEx)));
        }

        #endregion
    }
}