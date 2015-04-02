using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

namespace SharpLib.Texter.Editing
{
    internal class ImeSupport
    {
        #region Поля

        private readonly TextArea textArea;

        private IntPtr currentContext;

        private IntPtr defaultImeWnd;

        private HwndSource hwndSource;

        private bool isReadOnly;

        private IntPtr previousContext;

        private EventHandler requerySuggestedHandler;

        #endregion

        #region Конструктор

        public ImeSupport(TextArea textArea)
        {
            if (textArea == null)
            {
                throw new ArgumentNullException("textArea");
            }
            this.textArea = textArea;
            InputMethod.SetIsInputMethodSuspended(this.textArea, textArea.Options.EnableImeSupport);

            requerySuggestedHandler = OnRequerySuggested;
            CommandManager.RequerySuggested += requerySuggestedHandler;
            textArea.OptionChanged += TextAreaOptionChanged;
        }

        #endregion

        #region Методы

        private void OnRequerySuggested(object sender, EventArgs e)
        {
            UpdateImeEnabled();
        }

        private void TextAreaOptionChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "EnableImeSupport")
            {
                InputMethod.SetIsInputMethodSuspended(textArea, textArea.Options.EnableImeSupport);
                UpdateImeEnabled();
            }
        }

        public void OnGotKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            UpdateImeEnabled();
        }

        public void OnLostKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            if (e.OldFocus == textArea && currentContext != IntPtr.Zero)
            {
                ImeNativeWrapper.NotifyIme(currentContext);
            }
            ClearContext();
        }

        private void UpdateImeEnabled()
        {
            if (textArea.Options.EnableImeSupport && textArea.IsKeyboardFocused)
            {
                bool newReadOnly = !textArea.ReadOnlySectionProvider.CanInsert(textArea.Caret.Offset);
                if (hwndSource == null || isReadOnly != newReadOnly)
                {
                    ClearContext();
                    isReadOnly = newReadOnly;
                    CreateContext();
                }
            }
            else
            {
                ClearContext();
            }
        }

        private void ClearContext()
        {
            if (hwndSource != null)
            {
                ImeNativeWrapper.ImmAssociateContext(hwndSource.Handle, previousContext);
                ImeNativeWrapper.ImmReleaseContext(defaultImeWnd, currentContext);
                currentContext = IntPtr.Zero;
                defaultImeWnd = IntPtr.Zero;
                hwndSource.RemoveHook(WndProc);
                hwndSource = null;
            }
        }

        private void CreateContext()
        {
            hwndSource = (HwndSource)PresentationSource.FromVisual(textArea);
            if (hwndSource != null)
            {
                if (isReadOnly)
                {
                    defaultImeWnd = IntPtr.Zero;
                    currentContext = IntPtr.Zero;
                }
                else
                {
                    defaultImeWnd = ImeNativeWrapper.ImmGetDefaultIMEWnd(IntPtr.Zero);
                    currentContext = ImeNativeWrapper.ImmGetContext(defaultImeWnd);
                }
                previousContext = ImeNativeWrapper.ImmAssociateContext(hwndSource.Handle, currentContext);
                hwndSource.AddHook(WndProc);

                var threadMgr = ImeNativeWrapper.GetTextFrameworkThreadManager();
                if (threadMgr != null)
                {
                    threadMgr.SetFocus(IntPtr.Zero);
                }
            }
        }

        private IntPtr WndProc(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                case ImeNativeWrapper.WM_INPUTLANGCHANGE:

                    if (hwndSource != null)
                    {
                        ClearContext();
                        CreateContext();
                    }
                    break;
                case ImeNativeWrapper.WM_IME_COMPOSITION:
                    UpdateCompositionWindow();
                    break;
            }
            return IntPtr.Zero;
        }

        public void UpdateCompositionWindow()
        {
            if (currentContext != IntPtr.Zero)
            {
                ImeNativeWrapper.SetCompositionFont(hwndSource, currentContext, textArea);
                ImeNativeWrapper.SetCompositionWindow(hwndSource, currentContext, textArea);
            }
        }

        #endregion
    }
}