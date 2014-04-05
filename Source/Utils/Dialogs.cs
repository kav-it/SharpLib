// ****************************************************************************
//
// Имя файла    : 'Dialogs.cs'
// Заголовок    : Модуль диалогов загрузки/сохраниния/печати 
// Автор        : Крыцкий А.В.
// Контакты     : kav.it@mail.ru
// Дата         : 01/11/2012
//
// ****************************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Windows.Media;

using Microsoft.Win32;

namespace SharpLib
{

    #region Перечисление DialogFileTyp

    public enum DialogFileTyp
    {
        All = 0,

        Xml = 1,

        Txt = 2,

        Log = 3,

        Hex = 4,

        Sbin = 5,

        Exe = 6,

        Dll = 7,

        PcadLib = 8,

        Images = 9,

        File = 10,

        Backup = 11,

        Cpp_Source = 12,

        Cpp_Headers = 13,

        Cpp_Files = 14,

        Asm_Files = 15,
        //
        Wxml = 20,

        Pxml = 21,
    }

    #endregion Перечисление DialogFileTyp

    #region Перечисление DialogFolderType

    public enum DialogFolderType
    {
        SpecialFolder,

        Path,
    }

    #endregion Перечисление DialogFolderType

    #region Класс Dialogs

    public class Dialogs
    {
        #region Методы

        private static String GetFilter(DialogFileTyp typ, String filename = null)
        {
            switch (typ)
            {
                case DialogFileTyp.All:
                    return "Все файлы (*.*)|*.*";
                case DialogFileTyp.Xml:
                    return "Xml-файлы (*.xml)|*.xml";
                case DialogFileTyp.Txt:
                    return "Текстовые файлы (*.txt)|*.txt";
                case DialogFileTyp.Log:
                    return "Лог файлы (*.log)|*.log";
                case DialogFileTyp.Hex:
                    return "Hex-файлы (*.hex)|*.hex";
                case DialogFileTyp.Sbin:
                    return "Файлы прошивок приборов Стелс (*.sbin)|*.sbin";
                case DialogFileTyp.Exe:
                    return "Исполняемые файлы (*.exe)|*.exe";
                case DialogFileTyp.Dll:
                    return "Библиотеки (*.dll)|*.dll";
                case DialogFileTyp.PcadLib:
                    return "Библиотеки Pcad (*.lib)|*.lib";
                case DialogFileTyp.File:
                    return String.Format("Указанный файл ({0})|{0}", filename);
                case DialogFileTyp.Backup:
                    return "Файлы резервных копий (*.backup)|*.backup";
                case DialogFileTyp.Images:
                    return "PNG (.png)|*.png|JPEG (*jpg;*.jpeg)|*.jpg;*.jpeg|BMP (*.bmp)|*.bmp|GIF (*.gif)|*.gif|TIF (*.tif;*.tiff)|*.tif;*.tiff";
                case DialogFileTyp.Cpp_Source:
                    return "C/C++ код (*.cpp,*.c)|*.cpp;*.c";
                case DialogFileTyp.Cpp_Headers:
                    return "C/C++ заголовки (*.h,*.hpp)|*.h;*.hpp";
                case DialogFileTyp.Cpp_Files:
                    return "C/C++ файлы (*.cpp,*.c,*.h,*.hpp)|*.cpp;*.c;*.h;*.hpp";
                case DialogFileTyp.Asm_Files:
                    return "Asm файлы (*.asm)|*.asm";
                case DialogFileTyp.Wxml:
                    return "Файл workspace (*.wxml)|*.wxml";
                case DialogFileTyp.Pxml:
                    return "Файл проекта (*.pxml)|*.pxml";
            }

            return "Неизвестные файлы (*.*)|*.*";
        }

        public static String LoadFile(DialogFileTyp typ, String filename = null, String initDir = null)
        {
            OpenFileDialog dialog = new OpenFileDialog();

            dialog.Filter = GetFilter(typ, filename);
            dialog.FileName = filename;
            dialog.InitialDirectory = initDir;

            if (dialog.ShowDialog() == true)
                return dialog.FileName;

            return "";
        }

        public static String SaveFile(DialogFileTyp typ, String filename)
        {
            SaveFileDialog dialog = new SaveFileDialog();

            dialog.Filter = GetFilter(typ, filename);
            dialog.FileName = filename;

            if (dialog.ShowDialog() == true)
                return dialog.FileName;

            return "";
        }

        public static String SelectFile(DialogFileTyp typ, String title = null, String filename = null, String initDir = null)
        {
            if (title == null)
                title = "Выбор файла";

            OpenFileDialog dialog = new OpenFileDialog();

            dialog.Filter = GetFilter(typ, filename);
            dialog.FileName = filename;
            dialog.Title = title;

            if (dialog.ShowDialog() == true)
                return dialog.FileName;

            return "";
        }

        public static List<String> SelectFiles(List<DialogFileTyp> types, String title, String filename = null, String initDir = null)
        {
            String filter = "";

            foreach (DialogFileTyp typ in types)
                filter += GetFilter(typ) + "|";
            filter = filter.TrimEndEx("|");

            OpenFileDialog dialog = new OpenFileDialog();

            dialog.Title = title;
            dialog.FileName = filename;
            dialog.Filter = filter;
            dialog.Multiselect = true;
            dialog.InitialDirectory = initDir;

            List<String> result = new List<String>();

            if (dialog.ShowDialog() == true)
            {
                String[] arr = dialog.FileNames;

                result = arr.ToList();
            }

            return result;
        }

        /// <summary>
        /// Диалог выбора папки
        /// </summary>
        public static String SelectFolder(String textHeader = "", String startPath = "", Boolean browseFile = false)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();

            dialog.Title = textHeader;
            dialog.BrowseFiles = browseFile;
            dialog.SelectedPath = startPath;

            if (dialog.ShowDialog() == true)
                return dialog.SelectedPath;

            return null;
        }

        /// <summary>
        /// Сохранение картинки
        /// </summary>
        public static String SaveImage(ImageSource imageSource, String filename = "")
        {
            SaveFileDialog dialog = new SaveFileDialog();

            dialog.Filter = GetFilter(DialogFileTyp.Images, filename);
            dialog.FileName = filename.IsValid() ? filename : "image";

            if (dialog.ShowDialog() == true)
            {
                filename = dialog.FileName;

                Files.SaveImage(imageSource, filename, ImageTyp.Unknow);

                return filename;
            }

            return "";
        }

        #endregion
    }

    #endregion Класс Dialogs

    #region Класс FolderBrowserDialog

    public sealed class FolderBrowserDialog : CommonDialog
    {
        #region Поля

        [SecurityCritical]
        [SecuritySafeCritical]
        private NativeMethods.FolderBrowserOptions _dialogOptions;

        #endregion

        #region Свойства

        /// <summary>
        /// Тип корневой директории
        /// </summary>
        public DialogFolderType RootType { get; set; }

        /// <summary>
        /// Корневой путь
        /// </summary>
        public String RootPath { get; set; }

        /// <summary>
        /// Корневой путь системной директории
        /// </summary>
        public Environment.SpecialFolder RootSpecialFolder { get; set; }

        /// <summary>
        /// Выбранный путь
        /// </summary>
        public String SelectedPath { get; set; }

        /// <summary>
        /// Заголовок диалога
        /// </summary>
        public String Title { get; set; }

        /// <summary>
        /// Признак разрешения обозревателя файлов
        /// </summary>
        public Boolean BrowseFiles
        {
            get { return GetOption(NativeMethods.FolderBrowserOptions.BrowseFiles); }
            [SecurityCritical]
            set { SetOption(NativeMethods.FolderBrowserOptions.BrowseFiles, value); }
        }

        /// <summary>
        /// Признак разрешения редактирования пути
        /// </summary>
        public Boolean ShowEditBox
        {
            get { return GetOption(NativeMethods.FolderBrowserOptions.ShowEditBox); }
            [SecurityCritical]
            set { SetOption(NativeMethods.FolderBrowserOptions.ShowEditBox, value); }
        }

        /// <summary>
        /// Признак разрешения обозревателя сетевых папок
        /// </summary>
        public Boolean BrowseShares
        {
            get { return GetOption(NativeMethods.FolderBrowserOptions.BrowseShares); }
            [SecurityCritical]
            set { SetOption(NativeMethods.FolderBrowserOptions.BrowseShares, value); }
        }

        /// <summary>
        /// Признак разрешения отображения статуса
        /// </summary>
        public Boolean ShowStatusText
        {
            get { return GetOption(NativeMethods.FolderBrowserOptions.ShowStatusText); }
            [SecurityCritical]
            set { SetOption(NativeMethods.FolderBrowserOptions.ShowStatusText, value); }
        }

        /// <summary>
        /// Признак наличия проверки введенного пути
        /// </summary>
        public Boolean ValidateResult
        {
            get { return GetOption(NativeMethods.FolderBrowserOptions.ValidateResult); }
            [SecurityCritical]
            set { SetOption(NativeMethods.FolderBrowserOptions.ValidateResult, value); }
        }

        #endregion

        #region Конструктор

        [SecurityCritical]
        public FolderBrowserDialog()
        {
            Initialize();
        }

        #endregion

        #region Методы

        [SecurityCritical]
        private void Initialize()
        {
            RootType = DialogFolderType.SpecialFolder;
            RootSpecialFolder = Environment.SpecialFolder.Desktop;
            RootPath = String.Empty;
            Title = String.Empty;
            SelectedPath = String.Empty;

            // default options
            _dialogOptions = NativeMethods.FolderBrowserOptions.BrowseFiles
                             | NativeMethods.FolderBrowserOptions.ShowEditBox
                             | NativeMethods.FolderBrowserOptions.UseNewStyle
                             | NativeMethods.FolderBrowserOptions.BrowseShares
                             | NativeMethods.FolderBrowserOptions.ShowStatusText
                             | NativeMethods.FolderBrowserOptions.ValidateResult;
        }

        private Boolean GetOption(NativeMethods.FolderBrowserOptions option)
        {
            return ((_dialogOptions & option) != NativeMethods.FolderBrowserOptions.None);
        }

        [SecurityCritical]
        private void SetOption(NativeMethods.FolderBrowserOptions option, Boolean value)
        {
            if (value)
                _dialogOptions |= option;
            else
                _dialogOptions &= ~option;
        }

        [SecurityCritical]
        protected override Boolean RunDialog(IntPtr hwndOwner)
        {
            Boolean result = false;

            IntPtr pidlRoot = IntPtr.Zero,
                   pszPath = IntPtr.Zero,
                   pidlSelected = IntPtr.Zero;

            try
            {
                if (RootType == DialogFolderType.SpecialFolder)
                    NativeMethods.ShellGetFolderLocation(hwndOwner, (int)RootSpecialFolder, IntPtr.Zero, 0, out pidlRoot);
                else
                {
                    uint iAttribute;
                    NativeMethods.ShellParseDisplayName(RootPath, IntPtr.Zero, out pidlRoot, 0, out iAttribute);
                }

                NativeMethods.BrowseInfo browseInfo = new NativeMethods.BrowseInfo
                    {
                        HwndOwner = hwndOwner,
                        Root = pidlRoot,
                        DisplayName = new String(' ', 256),
                        Title = Title,
                        Flags = (uint)_dialogOptions,
                        LParam = 0,
                        Callback = HookProc
                    };

                // Show dialog
                pidlSelected = NativeMethods.ShellBrowseForFolder(ref browseInfo);

                if (pidlSelected != IntPtr.Zero)
                {
                    result = true;

                    pszPath = Marshal.AllocHGlobal((260 * Marshal.SystemDefaultCharSize));
                    NativeMethods.ShellGetPathFromIDList(pidlSelected, pszPath);

                    SelectedPath = Marshal.PtrToStringAuto(pszPath);
                }
            }
            finally
            {
                NativeMethods.IMalloc malloc = NativeMethods.GetShMalloc();

                if (pidlRoot != IntPtr.Zero)
                    malloc.Free(pidlRoot);

                if (pidlSelected != IntPtr.Zero)
                    malloc.Free(pidlSelected);

                if (pszPath != IntPtr.Zero)
                    Marshal.FreeHGlobal(pszPath);

                Marshal.ReleaseComObject(malloc);
            }

            return result;
        }

        [SecurityCritical]
        protected override IntPtr HookProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam)
        {
            switch (msg)
            {
                case BrowseForFolderMessages.BFFM_INITIALIZED:
                    // Установка начального пути
                    if (SelectedPath.IsValid())
                        NativeMethods.SendMessage(new HandleRef(null, hwnd), BrowseForFolderMessages.BFFM_SETSELECTIONW, 1, SelectedPath);
                    // Центрирование окна (не работает пока, т.к. выполняется изменение размеров после)
                    // NativeMethods.CenterTo(hwnd, IntPtr.Zero);
                    break;
                default:
                    return base.HookProc(hwnd, msg, wParam, lParam);
            }

            return (IntPtr)0;
        }

        [SecurityCritical]
        public override void Reset()
        {
            new System.Security.Permissions.FileIOPermission(System.Security.Permissions.PermissionState.Unrestricted).Demand();

            Initialize();
        }

        #endregion

        #region Класс BrowseForFolderMessages

        private static class BrowseForFolderMessages
        {
            // messages FROM the folder browser

            #region Константы

            public const int BFFM_ENABLEOK = 0x465;

            public const int BFFM_INITIALIZED = 1;

            public const int BFFM_IUNKNOWN = 5;

            public const int BFFM_SELCHANGED = 2;

            // messages TO the folder browser

            public const int BFFM_SETSELECTIONA = 0x466;

            public const int BFFM_SETSELECTIONW = 0x467;

            public const int BFFM_SETSTATUSTEXT = 0x464;

            public const int BFFM_VALIDATEFAILEDA = 3;

            public const int BFFM_VALIDATEFAILEDW = 4;

            #endregion
        }

        #endregion Класс BrowseForFolderMessages
    }

    #endregion Класс FolderBrowserDialog
}