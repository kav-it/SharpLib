using System;

namespace SharpLib.Wpf.Dialogs
{
    internal class VistaFileDialogEvents : Interop.IFileDialogEvents, Interop.IFileDialogControlEvents
    {
        #region ����

        private readonly VistaFileDialog _dialog;

        #endregion

        #region �����������

        internal VistaFileDialogEvents(VistaFileDialog dialog)
        {
            if (dialog == null)
            {
                throw new ArgumentNullException("dialog");
            }

            _dialog = dialog;
        }

        #endregion

        #region ������

        public Interop.HRESULT OnFileOk(Interop.IFileDialog pfd)
        {
            return _dialog.DoFileOk(pfd) ? Interop.HRESULT.S_OK : Interop.HRESULT.S_FALSE;
        }

        public Interop.HRESULT OnFolderChanging(Interop.IFileDialog pfd, Interop.IShellItem psiFolder)
        {
            return Interop.HRESULT.S_OK;
        }

        public void OnFolderChange(Interop.IFileDialog pfd)
        {
        }

        public void OnSelectionChange(Interop.IFileDialog pfd)
        {
        }

        public void OnShareViolation(Interop.IFileDialog pfd, Interop.IShellItem psi, out DialogNativeMethods.FDE_SHAREVIOLATION_RESPONSE pResponse)
        {
            pResponse = DialogNativeMethods.FDE_SHAREVIOLATION_RESPONSE.FDESVR_DEFAULT;
        }

        public void OnTypeChange(Interop.IFileDialog pfd)
        {
        }

        public void OnOverwrite(Interop.IFileDialog pfd, Interop.IShellItem psi, out DialogNativeMethods.FDE_OVERWRITE_RESPONSE pResponse)
        {
            pResponse = DialogNativeMethods.FDE_OVERWRITE_RESPONSE.FDEOR_DEFAULT;
        }

        public void OnItemSelected(Interop.IFileDialogCustomize pfdc, int dwIDCtl, int dwIDItem)
        {
        }

        public void OnButtonClicked(Interop.IFileDialogCustomize pfdc, int dwIDCtl)
        {
        }

        public void OnCheckButtonToggled(Interop.IFileDialogCustomize pfdc, int dwIDCtl, bool bChecked)
        {
        }

        public void OnControlActivating(Interop.IFileDialogCustomize pfdc, int dwIDCtl)
        {
        }

        #endregion
    }
}