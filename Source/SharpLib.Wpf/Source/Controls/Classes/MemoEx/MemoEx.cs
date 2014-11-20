using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace SharpLib.Wpf.Controls
{
    public class MemoEx : RichTextBoxEx
    {

        #region Конструктор

        static MemoEx()
        {
            // Переопределение стиля по умолчанию
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MemoEx), new FrameworkPropertyMetadata(typeof(MemoEx)));
        }

        #endregion

        #region Методы

        #endregion
    }
}