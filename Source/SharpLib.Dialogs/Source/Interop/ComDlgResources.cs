namespace SharpLib.Wpf.Dialogs.Interop
{
    internal static class ComDlgResources
    {
        #region ������������

        public enum ComDlgResourceId
        {
            OpenButton = 370,

            Open = 384,

            FileNotFound = 391,

            CreatePrompt = 402,

            ReadOnly = 427,

            ConfirmSaveAs = 435
        }

        #endregion

        #region ����

        private static readonly Win32Resources _resources = new Win32Resources("comdlg32.dll");

        #endregion

        #region ������

        public static string LoadString(ComDlgResourceId id)
        {
            return _resources.LoadString((uint)id);
        }

        public static string FormatString(ComDlgResourceId id, params string[] args)
        {
            return _resources.FormatString((uint)id, args);
        }

        #endregion
    }
}