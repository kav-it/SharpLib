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
        File = (1 << 0),

        /// <summary>
        /// Только 1 папка
        /// </summary>
        Folder = (1 << 1),

        /// <summary>
        /// Несколько элементов
        /// </summary>
        Many = (1 << 2),

        /// <summary>
        /// Все (и файлы и директории)
        /// </summary>
        All = File | Folder | Many
    }
}