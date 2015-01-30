namespace SharpLib.Wpf.Dialogs
{
    /// <summary>
    /// Диалоги в WPF-приложении
    /// </summary>
    public static class Dialog
    {
        #region Методы

        /// <summary>
        /// Выбор файла для загрузки
        /// </summary>
        public static string OpenFile(string filter)
        {
            var dialog = new VistaOpenFileDialog();
            dialog.Filter = filter;

            var result = dialog.ShowDialog();
            if (result.HasValue && result.Value)
            {
                return dialog.FileName;
            }

            return null;
        }

        /// <summary>
        /// Выбор файла для сохранения
        /// </summary>
        public static string SaveFile(string filter, string ext)
        {
            var dialog = new VistaSaveFileDialog();
            dialog.Filter = filter;
            dialog.DefaultExt = ext;

            var result = dialog.ShowDialog();
            if (result.HasValue && result.Value)
            {
                return dialog.FileName;
            }

            return null;
        }

        public static string SelectFolder(string title = null)
        {
            var dialog = new VistaFolderBrowserDialog();
            dialog.Description = title ?? "Выберите директорию";
            dialog.UseDescriptionForTitle = true;

            var result = dialog.ShowDialog();
            if (result.HasValue && result.Value)
            {
                return dialog.SelectedPath;
            }

            return null;
        }

        #endregion
    }
}