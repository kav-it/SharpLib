using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Windows.Forms;

using SharpLib.Native.Windows;

namespace SharpLib.WinForms.Dialogs
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
            using (var dialog = new OpenFileDialog())
            {
                dialog.Filter = filter.Value;
                dialog.FileName = filename;
                dialog.InitialDirectory = initDir;

                if (dialog.ShowDialog() == DialogResult.OK)
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
            using (var dialog = new SaveFileDialog())
            {
                dialog.Filter = filter.Value;
                dialog.FileName = filename;

                if (dialog.ShowDialog() == DialogResult.OK)
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

            using (var dialog = new OpenFileDialog())
            {

                dialog.Filter = filter.Value;
                dialog.FileName = filename;
                dialog.Title = title;

                if (dialog.ShowDialog() == DialogResult.OK)
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

            using (var dialog = new OpenFileDialog())
            {
                dialog.Title = title;
                dialog.FileName = filename;
                dialog.Filter = filter;
                dialog.Multiselect = true;
                dialog.InitialDirectory = initDir;

                var result = new List<string>();

                if (dialog.ShowDialog() == DialogResult.OK)
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
        public static string SelectFolder(string textHeader = "", string startPath = "", bool browseFile = false)
        {
            using (var dialog = new FolderBrowserDialog())
            {
                dialog.Title = textHeader;
                dialog.BrowseFiles = browseFile;
                dialog.SelectedPath = startPath;

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    return dialog.SelectedPath;
                }
            }

            return null;
        }

        #endregion

        #region Вложенный класс: FolderBrowserDialog

        internal sealed class FolderBrowserDialog : CommonDialog
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
                {
                    _dialogOptions |= option;
                }
                else
                {
                    _dialogOptions &= ~option;
                }
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
                    {
                        NativeMethods.ShellGetFolderLocation(hwndOwner, (int)RootSpecialFolder, IntPtr.Zero, 0, out pidlRoot);
                    }
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
                    {
                        malloc.Free(pidlRoot);
                    }

                    if (pidlSelected != IntPtr.Zero)
                    {
                        malloc.Free(pidlSelected);
                    }

                    if (pszPath != IntPtr.Zero)
                    {
                        Marshal.FreeHGlobal(pszPath);
                    }

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
                        {
                            NativeMethods.SendMessage(new HandleRef(null, hwnd), BrowseForFolderMessages.BFFM_SETSELECTIONW, 1, SelectedPath);
                        }
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

            #region Вложенный класс: BrowseForFolderMessages

            private static class BrowseForFolderMessages
            {
                #region Константы

                public const int BFFM_INITIALIZED = 1;

                public const int BFFM_SETSELECTIONW = 0x467;

                #endregion
            }

            #endregion
        }

        #endregion
    }
}