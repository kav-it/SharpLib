using System.ComponentModel;
using System.IO;
using System.Windows;

using Microsoft.Win32;

using SharpLib.Wpf.Dialogs.Interop;

namespace SharpLib.Wpf.Dialogs
{
    internal sealed class VistaSaveFileDialog : VistaFileDialog
    {
        #region Свойства

        public bool CreatePrompt
        {
            get
            {
                if (DownlevelDialog != null)
                {
                    return ((SaveFileDialog)DownlevelDialog).CreatePrompt;
                }
                return GetOption(DialogNativeMethods.FOS.FOS_CREATEPROMPT);
            }
            set
            {
                if (DownlevelDialog != null)
                {
                    ((SaveFileDialog)DownlevelDialog).CreatePrompt = value;
                }
                else
                {
                    SetOption(DialogNativeMethods.FOS.FOS_CREATEPROMPT, value);
                }
            }
        }

        public bool OverwritePrompt
        {
            get
            {
                if (DownlevelDialog != null)
                {
                    return ((SaveFileDialog)DownlevelDialog).OverwritePrompt;
                }
                return GetOption(DialogNativeMethods.FOS.FOS_OVERWRITEPROMPT);
            }
            set
            {
                if (DownlevelDialog != null)
                {
                    ((SaveFileDialog)DownlevelDialog).OverwritePrompt = value;
                }
                else
                {
                    SetOption(DialogNativeMethods.FOS.FOS_OVERWRITEPROMPT, value);
                }
            }
        }

        #endregion

        #region Конструктор

        internal VistaSaveFileDialog()
        {
            if (!IsVistaFileDialogSupported)
            {
                DownlevelDialog = new SaveFileDialog();
            }
        }

        #endregion

        #region Методы

        internal override void Reset()
        {
            base.Reset();
            if (DownlevelDialog == null)
            {
                OverwritePrompt = true;
            }
        }

        public Stream OpenFile()
        {
            if (DownlevelDialog != null)
            {
                return ((SaveFileDialog)DownlevelDialog).OpenFile();
            }
            string fileName = FileName;
            return new FileStream(fileName, FileMode.Create, FileAccess.ReadWrite);
        }

        protected override void OnFileOk(CancelEventArgs e)
        {
            if (DownlevelDialog == null)
            {
                if (CheckFileExists && !File.Exists(FileName))
                {
                    PromptUser(ComDlgResources.FormatString(ComDlgResources.ComDlgResourceId.FileNotFound, Path.GetFileName(FileName)), MessageBoxButton.OK, MessageBoxImage.Exclamation,
                        MessageBoxResult.OK);
                    e.Cancel = true;
                    return;
                }
                if (CreatePrompt && !File.Exists(FileName))
                {
                    if (
                        !PromptUser(ComDlgResources.FormatString(ComDlgResources.ComDlgResourceId.CreatePrompt, Path.GetFileName(FileName)), MessageBoxButton.YesNo, MessageBoxImage.Exclamation,
                            MessageBoxResult.No))
                    {
                        e.Cancel = true;
                        return;
                    }
                }
            }
            base.OnFileOk(e);
        }

        internal override IFileDialog CreateFileDialog()
        {
            return new NativeFileSaveDialog();
        }

        #endregion
    }
}