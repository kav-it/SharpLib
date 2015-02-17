using System.Windows;

namespace SharpLib.Wpf.Controls
{
    public class PathConversionEventArgs : RoutedEventArgs
    {
        #region Перечисления

        public enum ConversionMode
        {
            DisplayToEdit,

            EditToDisplay,
        }

        #endregion

        #region Свойства

        /// <summary>
        /// Отображаемое значение (пользовательское представление)
        /// </summary>
        public string DisplayPath { get; set; }

        /// <summary>
        /// Редактируемое значение
        /// </summary>
        public string EditPath { get; set; }

        public ConversionMode Mode { get; private set; }

        public object Root { get; private set; }

        #endregion

        #region Конструктор

        public PathConversionEventArgs(ConversionMode mode, string path, object root, RoutedEvent routedEvent)
            : base(routedEvent)
        {
            Mode = mode;
            DisplayPath = EditPath = path;
            Root = root;
        }

        #endregion
    }

    public delegate void PathConversionEventHandler(object sender, PathConversionEventArgs e);
}