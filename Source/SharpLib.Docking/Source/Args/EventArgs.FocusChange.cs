using System;

namespace SharpLib.Docking.Controls
{
    internal class FocusChangeEventArgs : EventArgs
    {
        #region Свойства

        public IntPtr GotFocusWinHandle { get; private set; }

        public IntPtr LostFocusWinHandle { get; private set; }

        #endregion

        #region Конструктор

        public FocusChangeEventArgs(IntPtr gotFocusWinHandle, IntPtr lostFocusWinHandle)
        {
            GotFocusWinHandle = gotFocusWinHandle;
            LostFocusWinHandle = lostFocusWinHandle;
        }

        #endregion
    }
}