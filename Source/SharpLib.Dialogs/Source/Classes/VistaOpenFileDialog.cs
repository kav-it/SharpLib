using System.IO;

using Microsoft.Win32;

using SharpLib.Wpf.Dialogs.Interop;

namespace SharpLib.Wpf.Dialogs
{
    internal sealed class VistaOpenFileDialog : VistaFileDialog
    {
        #region Константы

        private const int OPEN_DROP_DOWN_ID = 0x4002;

        private const int OPEN_ITEM_ID = 0x4003;

        private const int READ_ONLY_ITEM_ID = 0x4004;

        #endregion

        #region Поля

        private bool _readOnlyChecked;

        private bool _showReadOnly;

        #endregion

        #region Свойства

        public bool Multiselect
        {
            get
            {
                if (DownlevelDialog != null)
                {
                    return ((OpenFileDialog)DownlevelDialog).Multiselect;
                }
                return GetOption(DialogNativeMethods.FOS.FOS_ALLOWMULTISELECT);
            }
            set
            {
                if (DownlevelDialog != null)
                {
                    ((OpenFileDialog)DownlevelDialog).Multiselect = value;
                }

                SetOption(DialogNativeMethods.FOS.FOS_ALLOWMULTISELECT, value);
            }
        }

        public bool ShowReadOnly
        {
            get
            {
                if (DownlevelDialog != null)
                {
                    return ((OpenFileDialog)DownlevelDialog).ShowReadOnly;
                }
                return _showReadOnly;
            }
            set
            {
                if (DownlevelDialog != null)
                {
                    ((OpenFileDialog)DownlevelDialog).ShowReadOnly = value;
                }
                else
                {
                    _showReadOnly = value;
                }
            }
        }

        public bool ReadOnlyChecked
        {
            get
            {
                if (DownlevelDialog != null)
                {
                    return ((OpenFileDialog)DownlevelDialog).ReadOnlyChecked;
                }
                return _readOnlyChecked;
            }
            set
            {
                if (DownlevelDialog != null)
                {
                    ((OpenFileDialog)DownlevelDialog).ReadOnlyChecked = value;
                }
                else
                {
                    _readOnlyChecked = value;
                }
            }
        }

        #endregion

        #region Конструктор

        internal VistaOpenFileDialog()
        {
            if (!IsVistaFileDialogSupported)
            {
                DownlevelDialog = new OpenFileDialog();
            }
        }

        #endregion

        #region Методы

        internal override void Reset()
        {
            base.Reset();
            if (DownlevelDialog == null)
            {
                CheckFileExists = true;
                _showReadOnly = false;
                _readOnlyChecked = false;
            }
        }

        public Stream OpenFile()
        {
            if (DownlevelDialog != null)
            {
                return ((OpenFileDialog)DownlevelDialog).OpenFile();
            }
            string fileName = FileName;
            return new FileStream(fileName, FileMode.Open, FileAccess.Read);
        }

        internal override IFileDialog CreateFileDialog()
        {
            return new NativeFileOpenDialog();
        }

        internal override void SetDialogProperties(IFileDialog dialog)
        {
            base.SetDialogProperties(dialog);
            if (_showReadOnly)
            {
                var customize = (IFileDialogCustomize)dialog;
                customize.EnableOpenDropDown(OPEN_DROP_DOWN_ID);
                customize.AddControlItem(OPEN_DROP_DOWN_ID, OPEN_ITEM_ID, ComDlgResources.LoadString(ComDlgResources.ComDlgResourceId.OpenButton));
                customize.AddControlItem(OPEN_DROP_DOWN_ID, READ_ONLY_ITEM_ID, ComDlgResources.LoadString(ComDlgResources.ComDlgResourceId.ReadOnly));
            }
        }

        internal override void GetResult(IFileDialog dialog)
        {
            if (Multiselect)
            {
                IShellItemArray results;
                ((IFileOpenDialog)dialog).GetResults(out results);
                uint count;
                results.GetCount(out count);
                string[] fileNames = new string[count];
                for (uint x = 0; x < count; ++x)
                {
                    IShellItem item;
                    results.GetItemAt(x, out item);
                    string name;
                    item.GetDisplayName(DialogNativeMethods.SIGDN.SIGDN_FILESYSPATH, out name);
                    fileNames[x] = name;
                }
                FileNamesInternal = fileNames;
            }
            else
            {
                FileNamesInternal = null;
            }

            if (ShowReadOnly)
            {
                var customize = (IFileDialogCustomize)dialog;
                int selected;
                customize.GetSelectedControlItem(OPEN_DROP_DOWN_ID, out selected);
                _readOnlyChecked = (selected == READ_ONLY_ITEM_ID);
            }

            base.GetResult(dialog);
        }

        #endregion
    }
}