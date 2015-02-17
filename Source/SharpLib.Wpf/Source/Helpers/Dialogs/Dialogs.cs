using System.Collections.Generic;
using System.Linq;

namespace SharpLib.Wpf.Dialogs
{
    public class Dialog
    {
        #region Перечисления

        internal enum DialogFolderType
        {
            SpecialFolder,

            Path,
        }

        #endregion

        #region Методы

        /// <summary>
        /// Диалог "Открытие файла"
        /// </summary>
        public static string OpenFile(DialogFilter filter, string filename = null, string initDir = null)
        {
            using (var dialog = new System.Windows.Forms.OpenFileDialog())
            {
                dialog.Filter = filter.Value;
                dialog.FileName = filename;
                dialog.InitialDirectory = initDir;

                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    return dialog.FileName;
                }

                return string.Empty;
            }
        }

        /// <summary>
        /// Диалог "Сохранение файла"
        /// </summary>
        public static string SaveFile(DialogFilter filter, string filename)
        {
            using (var dialog = new System.Windows.Forms.SaveFileDialog())
            {
                dialog.Filter = filter.Value;
                dialog.FileName = filename;

                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    return dialog.FileName;
                }

                return string.Empty;
            }
        }

        /// <summary>
        /// Диалог "Выбор файла"
        /// </summary>
        public static string SelectFile(DialogFilter filter, string title = null, string filename = null, string initDir = null)
        {
            if (title == null)
            {
                title = "Выбор файла";
            }

            using (var dialog = new System.Windows.Forms.OpenFileDialog())
            {
                dialog.Filter = filter.Value;
                dialog.FileName = filename;
                dialog.Title = title;

                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    return dialog.FileName;
                }

                return string.Empty;
            }
        }

        /// <summary>
        /// Выбор файлов
        /// </summary>
        public static List<string> SelectFiles(List<DialogFilter> filters, string title, string filename = null, string initDir = null)
        {
            var filter = filters.Select(x => x.Value).JoinEx("|");

            using (var dialog = new System.Windows.Forms.OpenFileDialog())
            {
                dialog.Title = title;
                dialog.FileName = filename;
                dialog.Filter = filter;
                dialog.Multiselect = true;
                dialog.InitialDirectory = initDir;

                var result = new List<string>();

                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    var arr = dialog.FileNames;

                    result = arr.ToList();
                }

                return result;
            }
        }

        /// <summary>
        /// Диалог выбора папки
        /// </summary>
        public static string SelectFolder(string textHeader = null, string startPath = null)
        {
            var dialog = new CustomDialog(DialogCustomSelectMode.Folder, startPath, textHeader);

            if (dialog.ShowDialog() == true)
            {
                return dialog.SelectedFolders.FirstOrDefault();
            }

            return null;
        }

        /// <summary>
        /// Диалог выбора папки
        /// </summary>
        public static List<string> SelectFolders(string startPath = null, string textHeader = null)
        {
            var dialog = new CustomDialog(DialogCustomSelectMode.Folder | DialogCustomSelectMode.Many, startPath, textHeader);

            if (dialog.ShowDialog() == true)
            {
                return dialog.SelectedFolders;
            }

            return new List<string>();
        }

        /// <summary>
        /// Диалог выбора файлов и/или директорий
        /// </summary>
        public static List<string> SelectAny(string startPath = null, string textHeader = null)
        {
            var dialog = new CustomDialog(DialogCustomSelectMode.All, startPath, textHeader);

            if (dialog.ShowDialog() == true)
            {
                return dialog.SelectedItems.Select(x => x.Location).ToList();
            }

            return new List<string>();
        }

        #endregion
    }
}