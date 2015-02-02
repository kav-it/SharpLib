using System;
using System.Windows;

namespace SharpLib.Wpf
{
    /// <summary>
    /// Класс помощник работы с ресурсами WPF
    /// </summary>
    public static class Gui
    {
        /// <summary>
        /// Выполнение операции в UI-потоке
        /// </summary>
        public static void Invoke(Action action)
        {
            if (Application.Current != null)
            {
                Application.Current.Dispatcher.Invoke(action);    
            }
        }
    }
}
