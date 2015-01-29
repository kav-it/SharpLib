using System;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Interop;

using Microsoft.Win32;

using Ookii.Dialogs.Wpf.Interop;

using SharpLib;

namespace Ookii.Dialogs.Wpf
{
    [DefaultEvent("FileOk"), DefaultProperty("FileName")]
    public abstract class VistaFileDialog
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

        private NativeMethods.FOS _options;

        private Window _owner;

        private string _title;

        #endregion

        #region Свойства

        [Browsable(false)]
        public static bool IsVistaFileDialogSupported
        {
            get { return Env.IsWindowsVistaOrLater; }
        }

        [Description("A value indicating whether the dialog box automatically adds an extension to a file name if the user omits the extension."), Category("Behavior"), DefaultValue(true)]
        public bool AddExtension
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

        [Description("A value indicating whether the dialog box displays a warning if the user specifies a file name that does not exist."), Category("Behavior"), DefaultValue(false)]
        public virtual bool CheckFileExists
        {
            get
            {
                if (DownlevelDialog != null)
                {
                    return DownlevelDialog.CheckFileExists;
                }
                return GetOption(NativeMethods.FOS.FOS_FILEMUSTEXIST);
            }
            set
            {
                if (DownlevelDialog != null)
                {
                    DownlevelDialog.CheckFileExists = value;
                }
                else
                {
                    SetOption(NativeMethods.FOS.FOS_FILEMUSTEXIST, value);
                }
            }
        }

        [Description("A value indicating whether the dialog box displays a warning if the user specifies a path that does not exist."), DefaultValue(true), Category("Behavior")]
        public bool CheckPathExists
        {
            get
            {
                if (DownlevelDialog != null)
                {
                    return DownlevelDialog.CheckPathExists;
                }
                return GetOption(NativeMethods.FOS.FOS_PATHMUSTEXIST);
            }
            set
            {
                if (DownlevelDialog != null)
                {
                    DownlevelDialog.CheckPathExists = value;
                }
                else
                {
                    SetOption(NativeMethods.FOS.FOS_PATHMUSTEXIST, value);
                }
            }
        }

        [Category("Behavior"), DefaultValue(""), Description("The default file name extension.")]
        public string DefaultExt
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

        [Category("Behavior"),
         Description("A value indicating whether the dialog box returns the location of the file referenced by the shortcut or whether it returns the location of the shortcut (.lnk)."),
         DefaultValue(true)]
        public bool DereferenceLinks
        {
            get
            {
                if (DownlevelDialog != null)
                {
                    return DownlevelDialog.DereferenceLinks;
                }
                return !GetOption(NativeMethods.FOS.FOS_NODEREFERENCELINKS);
            }
            set
            {
                if (DownlevelDialog != null)
                {
                    DownlevelDialog.DereferenceLinks = value;
                }
                else
                {
                    SetOption(NativeMethods.FOS.FOS_NODEREFERENCELINKS, !value);
                }
            }
        }

        [DefaultValue(""), Category("Data"), Description("A string containing the file name selected in the file dialog box.")]
        public string FileName
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

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
        [Description("The file names of all selected files in the dialog box."), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string[] FileNames
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

        [Description("The current file name filter string, which determines the choices that appear in the \"Save as file type\" or \"Files of type\" box in the dialog box."), Category("Behavior"),
         Localizable(true), DefaultValue("")]
        public string Filter
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

        [Description("The index of the filter currently selected in the file dialog box."), Category("Behavior"), DefaultValue(1)]
        public int FilterIndex
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

        [Description("The initial directory displayed by the file dialog box."), DefaultValue(""), Category("Data")]
        public string InitialDirectory
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

        [DefaultValue(false), Description("A value indicating whether the dialog box restores the current directory before closing."), Category("Behavior")]
        public bool RestoreDirectory
        {
            get
            {
                if (DownlevelDialog != null)
                {
                    return DownlevelDialog.RestoreDirectory;
                }
                return GetOption(NativeMethods.FOS.FOS_NOCHANGEDIR);
            }
            set
            {
                if (DownlevelDialog != null)
                {
                    DownlevelDialog.RestoreDirectory = value;
                }
                else
                {
                    SetOption(NativeMethods.FOS.FOS_NOCHANGEDIR, value);
                }
            }
        }

        [Description("The file dialog box title."), Category("Appearance"), DefaultValue(""), Localizable(true)]
        public string Title
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

        [DefaultValue(true), Category("Behavior"), Description("A value indicating whether the dialog box accepts only valid Win32 file names.")]
        public bool ValidateNames
        {
            get
            {
                if (DownlevelDialog != null)
                {
                    return DownlevelDialog.ValidateNames;
                }
                return !GetOption(NativeMethods.FOS.FOS_NOVALIDATE);
            }
            set
            {
                if (DownlevelDialog != null)
                {
                    DownlevelDialog.ValidateNames = value;
                }
                else
                {
                    SetOption(NativeMethods.FOS.FOS_NOVALIDATE, !value);
                }
            }
        }

        [Browsable(false)]
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

        [Description("Event raised when the user clicks on the Open or Save button on a file dialog box."), Category("Action")]
        public event CancelEventHandler FileOk;

        #endregion

        #region Конструктор

        protected VistaFileDialog()
        {
            Reset();
        }

        #endregion

        #region Методы

        public virtual void Reset()
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

        public bool? ShowDialog()
        {
            return ShowDialog(null);
        }

        public bool? ShowDialog(Window owner)
        {
            _owner = owner;
            if (DownlevelDialog != null)
            {
                return DownlevelDialog.ShowDialog(owner);
            }
            IntPtr ownerHandle = owner == null ? NativeMethods.GetActiveWindow() : new WindowInteropHelper(owner).Handle;
            return RunFileDialog(ownerHandle);
        }

        internal void SetOption(NativeMethods.FOS option, bool value)
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

        internal bool GetOption(NativeMethods.FOS option)
        {
            return (_options & option) != 0;
        }

        internal virtual void GetResult(IFileDialog dialog)
        {
            if (!GetOption(NativeMethods.FOS.FOS_ALLOWMULTISELECT))
            {
                _fileNames = new string[1];
                IShellItem result;
                dialog.GetResult(out result);
                result.GetDisplayName(NativeMethods.SIGDN.SIGDN_FILESYSPATH, out _fileNames[0]);
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
                    dialog.SetFolder(NativeMethods.CreateItemFromParsingName(parent));
                    dialog.SetFileName(folder);
                }
            }

            if (!string.IsNullOrEmpty(_filter))
            {
                string[] filterElements = _filter.Split(new[] { '|' });
                NativeMethods.COMDLG_FILTERSPEC[] filter = new NativeMethods.COMDLG_FILTERSPEC[filterElements.Length / 2];
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
                IShellItem item = NativeMethods.CreateItemFromParsingName(_initialDirectory);
                dialog.SetDefaultFolder(item);
            }

            if (!string.IsNullOrEmpty(_title))
            {
                dialog.SetTitle(_title);
            }

            dialog.SetOptions((_options | NativeMethods.FOS.FOS_FORCEFILESYSTEM));
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