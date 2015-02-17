using System;

namespace SharpLib.Wpf.Dialogs
{
    /// <summary>
    /// Режим Selection 
    /// </summary>
    [Flags]
    internal enum DialogCustomSelectMode
    {
        /// <summary>
        /// Неопределено
        /// </summary>
        None = 0,

        /// <summary>
        /// Только 1-файл
        /// </summary>
        SingleFile = (1 << 0),

        /// <summary>
        /// Только 1 папка
        /// </summary>
        SingleFolder = (1 << 1),

        /// <summary>
        /// Несколько папок
        /// </summary>
        Folders = (1 << 2),

        /// <summary>
        /// Несколько файлов
        /// </summary>
        Files = (1 << 3),

        /// <summary>
        /// Все (и файлы и директории)
        /// </summary>
        All = Folders | Files
    }
}