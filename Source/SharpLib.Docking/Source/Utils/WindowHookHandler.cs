using System;

namespace SharpLib.Docking.Controls
{
    internal class WindowHookHandler
    {
        #region Поля

        private readonly ReentrantFlag _insideActivateEvent;

        private Win32Helper.HookProc _hookProc;

        private IntPtr _windowHook;

        #endregion

        #region События

        public event EventHandler<FocusChangeEventArgs> FocusChanged;

        #endregion

        #region Конструктор

        public WindowHookHandler()
        {
            _insideActivateEvent = new ReentrantFlag();
        }

        #endregion

        #region Методы

        public void Attach()
        {
            _hookProc = HookProc;
            _windowHook = Win32Helper.SetWindowsHookEx(
                Win32Helper.HookType.WH_CBT,
                _hookProc,
                IntPtr.Zero,
                (int)Win32Helper.GetCurrentThreadId());
        }

        public void Detach()
        {
            Win32Helper.UnhookWindowsHookEx(_windowHook);
        }

        public int HookProc(int code, IntPtr wParam, IntPtr lParam)
        {
            if (code == Win32Helper.HCBT_SETFOCUS)
            {
                if (FocusChanged != null)
                {
                    FocusChanged(this, new FocusChangeEventArgs(wParam, lParam));
                }
            }
            else if (code == Win32Helper.HCBT_ACTIVATE)
            {
                if (_insideActivateEvent.CanEnter)
                {
                    using (_insideActivateEvent.Enter())
                    {
                    }
                }
            }

            return Win32Helper.CallNextHookEx(_windowHook, code, wParam, lParam);
        }

        #endregion
    }
}