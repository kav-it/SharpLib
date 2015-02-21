using System;

namespace SharpLib.Docking.Controls
{
    internal class WindowActivateEventArgs : EventArgs
    {
        #region Свойства

        public IntPtr HwndActivating { get; private set; }

        #endregion

        #region Конструктор

        public WindowActivateEventArgs(IntPtr hwndActivating)
        {
            HwndActivating = hwndActivating;
        }

        #endregion
    }
}