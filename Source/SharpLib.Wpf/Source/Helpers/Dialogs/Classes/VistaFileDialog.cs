using System;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Interop;

using Microsoft.Win32;

using SharpLib.Wpf.Dialogs.Interop;

namespace SharpLib.Wpf.Dialogs
{
    internal abstract class VistaFileDialog
    {
        #region Константы

        internal const int HELP_BUTTON_ID = 0x4001;

        #endregion

        #region Поля

        private bool _addExtension;

        private string _defaultExt;

        private FileDialog _downlevelDialog;

        private string[] _fileNames;

        private string _filter;

        private int _filterIndex;

        private string _initialDirectory;

        private DialogNativeMethods.FOS _options;

        private Window _owner;

        private string _title;

        #endregion

        #region Свойства

        internal static bool IsVistaFileDialogSupported
        {
            get { return Env.IsWindowsVistaOrLater; }
        }

        internal bool AddExtension
        {
            get
            {
                if (DownlevelDialog != null)
                {
                    return DownlevelDialog.AddExtension;
                }
                return _addExtension;
            }
            set
            {
                if (DownlevelDialog != null)
                {
                    DownlevelDialog.AddExtension = value;
                }
                else
                {
                    _addExtension = value;
                }
            }
        }

        internal virtual bool CheckFileExists
        {
            get
            {
                if (DownlevelDialog != null)
                {
                    return DownlevelDialog.CheckFileExists;
                }
                return GetOption(DialogNativeMethods.FOS.FOS_FILEMUSTEXIST);
            }
            set
            {
                if (DownlevelDialog != null)
                {
                    DownlevelDialog.CheckFileExists = value;
                }
                else
                {
                    SetOption(DialogNativeMethods.FOS.FOS_FILEMUSTEXIST, value);
                }
            }
        }

        internal bool CheckPathExists
        {
            get
            {
                if (DownlevelDialog != null)
                {
                    return DownlevelDialog.CheckPathExists;
                }
                return GetOption(DialogNativeMethods.FOS.FOS_PATHMUSTEXIST);
            }
            set
            {
                if (DownlevelDialog != null)
                {
                    DownlevelDialog.CheckPathExists = value;
                }
                else
                {
                    SetOption(DialogNativeMethods.FOS.FOS_PATHMUSTEXIST, value);
                }
            }
        }

        internal string DefaultExt
        {
            get
            {
                if (DownlevelDialog != null)
                {
                    return DownlevelDialog.DefaultExt;
                }
                return _defaultExt ?? string.Empty;
            }
            set
            {
                if (DownlevelDialog != null)
                {
                    DownlevelDialog.DefaultExt = value;
                }
                else
                {
                    if (value != null)
                    {
                        if (value.StartsWith(".", StringComparison.CurrentCulture))
                        {
                            value = value.Substring(1);
                        }
                        else if (value.Length == 0)
                        {
                            value = null;
                        }
                    }

                    _defaultExt = value;
                }
            }
        }

        internal bool DereferenceLinks
        {
            get
            {
                if (DownlevelDialog != null)
                {
                    return DownlevelDialog.DereferenceLinks;
                }
                return !GetOption(DialogNativeMethods.FOS.FOS_NODEREFERENCELINKS);
            }
            set
            {
                if (DownlevelDialog != null)
                {
                    DownlevelDialog.DereferenceLinks = value;
                }
                else
                {
                    SetOption(DialogNativeMethods.FOS.FOS_NODEREFERENCELINKS, !value);
                }
            }
        }

        internal string FileName
        {
            get
            {
                if (DownlevelDialog != null)
                {
                    return DownlevelDialog.FileName;
                }

                if (_fileNames == null || _fileNames.Length == 0 || string.IsNullOrEmpty(_fileNames[0]))
                {
                    return string.Empty;
                }
                return _fileNames[0];
            }
            set
            {
                if (DownlevelDialog != null)
                {
                    DownlevelDialog.FileName = value;
                }
                _fileNames = new string[1];
                _fileNames[0] = value;
            }
        }

        internal string[] FileNames
        {
            get
            {
                if (DownlevelDialog != null)
                {
                    return DownlevelDialog.FileNames;
                }
                return FileNamesInternal;
            }
        }

        internal string Filter
        {
            get
            {
                if (DownlevelDialog != null)
                {
                    return DownlevelDialog.Filter;
                }
                return _filter;
            }
            set
            {
                if (DownlevelDialog != null)
                {
                    DownlevelDialog.Filter = value;
                }
                else
                {
                    if (value != _filter)
                    {
                        if (!string.IsNullOrEmpty(value))
                        {
                            string[] filterElements = value.Split(new[] { '|' });
                            if (filterElements.Length % 2 != 0)
                            {
                                throw new ArgumentException("InvalidFilterString");
                            }
                        }
                        else
                        {
                            value = null;
                        }
                        _filter = value;
                    }
                }
            }
        }

        internal int FilterIndex
        {
            get
            {
                if (DownlevelDialog != null)
                {
                    return DownlevelDialog.FilterIndex;
                }
                return _filterIndex;
            }
            set
            {
                if (DownlevelDialog != null)
                {
                    DownlevelDialog.FilterIndex = value;
                }
                else
                {
                    _filterIndex = value;
                }
            }
        }

        internal string InitialDirectory
        {
            get
            {
                if (DownlevelDialog != null)
                {
                    return DownlevelDialog.InitialDirectory;
                }

                if (_initialDirectory != null)
                {
                    return _initialDirectory;
                }
                return string.Empty;
            }
            set
            {
                if (DownlevelDialog != null)
                {
                    DownlevelDialog.InitialDirectory = value;
                }
                else
                {
                    _initialDirectory = value;
                }
            }
        }

        internal bool RestoreDirectory
        {
            get
            {
                if (DownlevelDialog != null)
                {
                    return DownlevelDialog.RestoreDirectory;
                }
                return GetOption(DialogNativeMethods.FOS.FOS_NOCHANGEDIR);
            }
            set
            {
                if (DownlevelDialog != null)
                {
                    DownlevelDialog.RestoreDirectory = value;
                }
                else
                {
                    SetOption(DialogNativeMethods.FOS.FOS_NOCHANGEDIR, value);
                }
            }
        }

        internal string Title
        {
            get
            {
                if (DownlevelDialog != null)
                {
                    return DownlevelDialog.Title;
                }
                if (_title != null)
                {
                    return _title;
                }
                return string.Empty;
            }
            set
            {
                if (DownlevelDialog != null)
                {
                    DownlevelDialog.Title = value;
                }
                else
                {
                    _title = value;
                }
            }
        }

        internal bool ValidateNames
        {
            get
            {
                if (DownlevelDialog != null)
                {
                    return DownlevelDialog.ValidateNames;
                }
                return !GetOption(DialogNativeMethods.FOS.FOS_NOVALIDATE);
            }
            set
            {
                if (DownlevelDialog != null)
                {
                    DownlevelDialog.ValidateNames = value;
                }
                else
                {
                    SetOption(DialogNativeMethods.FOS.FOS_NOVALIDATE, !value);
                }
            }
        }

        protected FileDialog DownlevelDialog
        {
            get { return _downlevelDialog; }
            set
            {
                _downlevelDialog = value;
                if (value != null)
                {
                    value.FileOk += DownlevelDialog_FileOk;
                }
            }
        }

        internal string[] FileNamesInternal
        {
            private get
            {
                if (_fileNames == null)
                {
                    return new string[0];
                }
                return (string[])_fileNames.Clone();
            }
            set { _fileNames = value; }
        }

        #endregion

        #region События

        internal event CancelEventHandler FileOk;

        #endregion

        #region Конструктор

        protected VistaFileDialog()
        {
            // ReSharper disable DoNotCallOverridableMethodsInConstructor
            Reset();
            // ReSharper restore DoNotCallOverridableMethodsInConstructor
        }

        #endregion

        #region Методы

        internal virtual void Reset()
        {
            if (DownlevelDialog != null)
            {
                DownlevelDialog.Reset();
            }
            else
            {
                _fileNames = null;
                _filter = null;
                _filterIndex = 1;
                _addExtension = true;
                _defaultExt = null;
                _options = 0;
                _title = null;
                CheckPathExists = true;
            }
        }

        internal bool? ShowDialog()
        {
            return ShowDialog(null);
        }

        internal bool? ShowDialog(Window owner)
        {
            _owner = owner;
            if (DownlevelDialog != null)
            {
                return DownlevelDialog.ShowDialog(owner);
            }
            IntPtr ownerHandle = owner == null ? DialogNativeMethods.GetActiveWindow() : new WindowInteropHelper(owner).Handle;
            return RunFileDialog(ownerHandle);
        }

        internal void SetOption(DialogNativeMethods.FOS option, bool value)
        {
            if (value)
            {
                _options |= option;
            }
            else
            {
                _options &= ~option;
            }
        }

        internal bool GetOption(DialogNativeMethods.FOS option)
        {
            return (_options & option) != 0;
        }

        internal virtual void GetResult(IFileDialog dialog)
        {
            if (!GetOption(DialogNativeMethods.FOS.FOS_ALLOWMULTISELECT))
            {
                _fileNames = new string[1];
                IShellItem result;
                dialog.GetResult(out result);
                result.GetDisplayName(DialogNativeMethods.SIGDN.SIGDN_FILESYSPATH, out _fileNames[0]);
            }
        }

        protected virtual void OnFileOk(CancelEventArgs e)
        {
            CancelEventHandler handler = FileOk;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        internal bool PromptUser(string text, MessageBoxButton buttons, MessageBoxImage icon, MessageBoxResult defaultResult)
        {
            string caption = string.IsNullOrEmpty(_title)
                ? (this is VistaOpenFileDialog ? ComDlgResources.LoadString(ComDlgResources.ComDlgResourceId.Open) : ComDlgResources.LoadString(ComDlgResources.ComDlgResourceId.ConfirmSaveAs))
                : _title;
            MessageBoxOptions options = 0;
            if (System.Threading.Thread.CurrentThread.CurrentUICulture.TextInfo.IsRightToLeft)
            {
                options |= MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading;
            }
            return MessageBox.Show(_owner, text, caption, buttons, icon, defaultResult, options) == MessageBoxResult.Yes;
        }

        internal virtual void SetDialogProperties(IFileDialog dialog)
        {
            uint cookie;
            dialog.Advise(new VistaFileDialogEvents(this), out cookie);

            if (!(_fileNames == null || _fileNames.Length == 0 || string.IsNullOrEmpty(_fileNames[0])))
            {
                string parent = Path.GetDirectoryName(_fileNames[0]);
                if (parent == null || !Directory.Exists(parent))
                {
                    dialog.SetFileName(_fileNames[0]);
                }
                else
                {
                    string folder = Path.GetFileName(_fileNames[0]);
                    dialog.SetFolder(DialogNativeMethods.CreateItemFromParsingName(parent));
                    dialog.SetFileName(folder);
                }
            }

            if (!string.IsNullOrEmpty(_filter))
            {
                string[] filterElements = _filter.Split(new[] { '|' });
                DialogNativeMethods.COMDLG_FILTERSPEC[] filter = new DialogNativeMethods.COMDLG_FILTERSPEC[filterElements.Length / 2];
                for (int x = 0; x < filterElements.Length; x += 2)
                {
                    filter[x / 2].pszName = filterElements[x];
                    filter[x / 2].pszSpec = filterElements[x + 1];
                }
                dialog.SetFileTypes((uint)filter.Length, filter);

                if (_filterIndex > 0 && _filterIndex <= filter.Length)
                {
                    dialog.SetFileTypeIndex((uint)_filterIndex);
                }
            }

            if (_addExtension && !string.IsNullOrEmpty(_defaultExt))
            {
                dialog.SetDefaultExtension(_defaultExt);
            }

            if (!string.IsNullOrEmpty(_initialDirectory))
            {
                IShellItem item = DialogNativeMethods.CreateItemFromParsingName(_initialDirectory);
                dialog.SetDefaultFolder(item);
            }

            if (!string.IsNullOrEmpty(_title))
            {
                dialog.SetTitle(_title);
            }

            dialog.SetOptions((_options | DialogNativeMethods.FOS.FOS_FORCEFILESYSTEM));
        }

        internal abstract IFileDialog CreateFileDialog();

        internal bool DoFileOk(IFileDialog dialog)
        {
            GetResult(dialog);

            CancelEventArgs e = new CancelEventArgs();
            OnFileOk(e);
            return !e.Cancel;
        }

        private bool RunFileDialog(IntPtr hwndOwner)
        {
            IFileDialog dialog = null;
            try
            {
                dialog = CreateFileDialog();
                SetDialogProperties(dialog);
                int result = dialog.Show(hwndOwner);
                if (result < 0)
                {
                    if ((uint)result == (uint)HRESULT.ERROR_CANCELLED)
                    {
                        return false;
                    }
                    else
                    {
                        throw System.Runtime.InteropServices.Marshal.GetExceptionForHR(result);
                    }
                }
                return true;
            }
            finally
            {
                if (dialog != null)
                {
                    System.Runtime.InteropServices.Marshal.FinalReleaseComObject(dialog);
                }
            }
        }

        private void DownlevelDialog_FileOk(object sender, CancelEventArgs e)
        {
            OnFileOk(e);
        }

        #endregion
    }
}