namespace SharpLib.Wpf.Dialogs
{
    /// <summary>
    /// Фильтр файлов в диалогах
    /// </summary>
    public class DialogFilter
    {
        #region Поля

        public static DialogFilter Any = new DialogFilter("Все файлы", "*.*");

        public static DialogFilter Dll = new DialogFilter("Библиотеки", "*.dll");

        public static DialogFilter Exe = new DialogFilter("Исполняемые файлы", "*.exe");

        public static DialogFilter Hex = new DialogFilter("Hex-файлы", "*.hex");

        public static DialogFilter Images = new DialogFilter("PNG (.png)|*.png|JPEG (*jpg;*.jpeg)|*.jpg;*.jpeg|BMP (*.bmp)|*.bmp|GIF (*.gif)|*.gif|TIF (*.tif;*.tiff)|*.tif;*.tiff");

        public static DialogFilter Log = new DialogFilter("Лог файлы", "*.log");

        public static DialogFilter Txt = new DialogFilter("Текстовые файлы", "*.txt");

        public static DialogFilter Xml = new DialogFilter("Xml-файлы", "*.xml");

        #endregion

        #region Свойства

        /// <summary>
        /// Полный текст фильтра (Например: 'C/C++ заголовки (*.h,*.hpp)|*.h;*.hpp')
        /// </summary>
        public string Value { get; private set; }

        #endregion

        #region Конструктор

        public DialogFilter(string filterText)
        {
            Value = filterText;
        }

        public DialogFilter(string caption, string ext) : this(string.Format("{0} ({1})|{1}", caption, ext))
        {
        }

        #endregion

        /// <summary>
        /// Создание объекта фильтра по маске (*.exe => new DialogFilter("Файл (*.exe)|*.exe))
        /// </summary>
        public static implicit operator DialogFilter(string mask)
        {
            return new DialogFilter(string.Format("Файл ({0})|{0}", mask));
        }
    }
}