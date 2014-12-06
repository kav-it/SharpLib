﻿using System.Windows;
using System.Windows.Controls;

namespace SharpLib.Wpf.Controls
{
    public class ButtonEx : Button
    {
        #region Конструктор

        static ButtonEx()
        {
            // Переопределение стиля по умолчанию
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ButtonEx), new FrameworkPropertyMetadata(typeof(ButtonEx)));
        }

        #endregion
    }
}