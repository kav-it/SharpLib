using System;
using System.Linq;
using System.Windows;

namespace SharpLib.Wpf
{
    /// <summary>
    /// Класс помощник работы с ресурсами WPF
    /// </summary>
    public static class Gui
    {
        #region Методы

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

        public static Window GetActiveWindow()
        {
            return Application.Current.Windows
                .OfType<Window>()
                .SingleOrDefault(x => x.IsActive);
        }

        #endregion
    }
}