using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Interop;

using SharpLib.Wpf.Dialogs.Interop;

namespace SharpLib.Wpf.Dialogs
{
    internal sealed class VistaFolderBrowserDialog
    {
        #region Поля

        private string _description;

        private string _selectedPath;

        #endregion

        #region Свойства

        public static bool IsVistaFolderDialogSupported
        {
            get { return Env.IsWindowsVistaOrLater; }
        }

        public string Description
        {
            get { return _description ?? string.Empty; }
            set { _description = value; }
        }

        public Environment.SpecialFolder RootFolder { get; set; }

        public string SelectedPath
        {
            get { return _selectedPath ?? string.Empty; }
            set { _selectedPath = value; }
        }

        public bool ShowNewFolderButton { get; set; }

        public bool UseDescriptionForTitle { get; set; }

        #endregion

        #region Конструктор

        internal VistaFolderBrowserDialog()
        {
            Reset();
        }

        #endregion

        #region Методы

        public void Reset()
        {
            _description = string.Empty;
            UseDescriptionForTitle = false;
            _selectedPath = string.Empty;
            RootFolder = Environment.SpecialFolder.Desktop;
            ShowNewFolderButton = true;
        }

        public bool? ShowDialog()
        {
            return ShowDialog(null);
        }

        public bool? ShowDialog(Window owner)
        {
            IntPtr ownerHandle = owner == null ? DialogNativeMethods.GetActiveWindow() : new WindowInteropHelper(owner).Handle;
            return IsVistaFolderDialogSupported ? RunDialog(ownerHandle) : RunDialogDownlevel(ownerHandle);
        }

        private bool RunDialog(IntPtr owner)
        {
            IFileDialog dialog = null;
            try
            {
                dialog = new NativeFileOpenDialog();
                SetDialogProperties(dialog);
                int result = dialog.Show(owner);
                if (result < 0)
                {
                    if ((uint)result == (uint)HRESULT.ERROR_CANCELLED)
                    {
                        return false;
                    }
                    else
                    {
                        throw Marshal.GetExceptionForHR(result);
                    }
                }
                GetResult(dialog);
                return true;
            }
            finally
            {
                if (dialog != null)
                {
                    Marshal.FinalReleaseComObject(dialog);
                }
            }
        }

        private bool RunDialogDownlevel(IntPtr owner)
        {
            IntPtr rootItemIdList = IntPtr.Zero;
            IntPtr resultItemIdList = IntPtr.Zero;
            if (DialogNativeMethods.SHGetSpecialFolderLocation(owner, RootFolder, ref rootItemIdList) != 0)
            {
                if (DialogNativeMethods.SHGetSpecialFolderLocation(owner, 0, ref rootItemIdList) != 0)
                {
                    throw new InvalidOperationException("FolderBrowserDialogNoRootFolder");
                }
            }
            try
            {
                DialogNativeMethods.BROWSEINFO info = new DialogNativeMethods.BROWSEINFO();
                info._hwndOwner = owner;
                info._lpfn = BrowseCallbackProc;
                info._lpszTitle = Description;
                info._pidlRoot = rootItemIdList;
                info._pszDisplayName = new string('\0', 260);
                info._ulFlags = DialogNativeMethods.BrowseInfoFlags.NewDialogStyle | DialogNativeMethods.BrowseInfoFlags.ReturnOnlyFsDirs;
                if (!ShowNewFolderButton)
                {
                    info._ulFlags |= DialogNativeMethods.BrowseInfoFlags.NoNewFolderButton;
                }
                resultItemIdList = DialogNativeMethods.SHBrowseForFolder(ref info);
                if (resultItemIdList != IntPtr.Zero)
                {
                    StringBuilder path = new StringBuilder(260);
                    DialogNativeMethods.SHGetPathFromIDList(resultItemIdList, path);
                    SelectedPath = path.ToString();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            finally
            {
                
                IMalloc malloc = DialogNativeMethods.SHGetMalloc();
                malloc.Free(rootItemIdList);
                Marshal.ReleaseComObject(malloc);
                Marshal.FreeCoTaskMem(resultItemIdList);
            }
        }

        private void SetDialogProperties(IFileDialog dialog)
        {
            if (!string.IsNullOrEmpty(_description))
            {
                if (UseDescriptionForTitle)
                {
                    dialog.SetTitle(_description);
                }
                else
                {
                    var customize = (IFileDialogCustomize)dialog;
                    customize.AddText(0, _description);
                }
            }

            dialog.SetOptions(DialogNativeMethods.FOS.FOS_PICKFOLDERS | DialogNativeMethods.FOS.FOS_FORCEFILESYSTEM | DialogNativeMethods.FOS.FOS_FILEMUSTEXIST);

            if (!string.IsNullOrEmpty(_selectedPath))
            {
                string parent = Path.GetDirectoryName(_selectedPath);
                if (parent == null || !Directory.Exists(parent))
                {
                    dialog.SetFileName(_selectedPath);
                }
                else
                {
                    string folder = Path.GetFileName(_selectedPath);
                    dialog.SetFolder(DialogNativeMethods.CreateItemFromParsingName(parent));
                    dialog.SetFileName(folder);
                }
            }
        }

        private void GetResult(IFileDialog dialog)
        {
            IShellItem item;
            dialog.GetResult(out item);
            item.GetDisplayName(DialogNativeMethods.SIGDN.SIGDN_FILESYSPATH, out _selectedPath);
        }

        private int BrowseCallbackProc(IntPtr hwnd, DialogNativeMethods.FolderBrowserDialogMessage msg, IntPtr lParam, IntPtr wParam)
        {
            switch (msg)
            {
                case DialogNativeMethods.FolderBrowserDialogMessage.Initialized:
                    if (SelectedPath.Length != 0)
                    {
                        DialogNativeMethods.SendMessage(hwnd, DialogNativeMethods.FolderBrowserDialogMessage.SetSelection, new IntPtr(1), SelectedPath);
                    }
                    break;
                case DialogNativeMethods.FolderBrowserDialogMessage.SelChanged:
                    if (lParam != IntPtr.Zero)
                    {
                        StringBuilder path = new StringBuilder(260);
                        bool validPath = DialogNativeMethods.SHGetPathFromIDList(lParam, path);
                        DialogNativeMethods.SendMessage(hwnd, DialogNativeMethods.FolderBrowserDialogMessage.EnableOk, IntPtr.Zero, validPath ? new IntPtr(1) : IntPtr.Zero);
                    }
                    break;
            }
            return 0;
        }

        #endregion
    }
}