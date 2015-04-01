using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

using SharpLib.Notepad.Rendering;
using SharpLib.Notepad.Utils;

using Draw = System.Drawing;

namespace SharpLib.Notepad.Editing
{
    internal static class ImeNativeWrapper
    {
        #region Константы

        private const int CPS_CANCEL = 0x4;

        private const int GCS_COMPSTR = 0x0008;

        private const int NI_COMPOSITIONSTR = 0x15;

        public const int WM_IME_COMPOSITION = 0x10F;

        public const int WM_IME_SETCONTEXT = 0x281;

        public const int WM_INPUTLANGCHANGE = 0x51;

        #endregion

        #region Поля

        private static readonly Rect EMPTY_RECT = new Rect(0, 0, 0, 0);

        [ThreadStatic]
        private static ITfThreadMgr textFrameworkThreadMgr;

        [ThreadStatic]
        private static bool textFrameworkThreadMgrInitialized;

        #endregion

        #region Методы

        [DllImport("imm32.dll")]
        public static extern IntPtr ImmAssociateContext(IntPtr hWnd, IntPtr hIMC);

        [DllImport("imm32.dll")]
        internal static extern IntPtr ImmGetContext(IntPtr hWnd);

        [DllImport("imm32.dll")]
        internal static extern IntPtr ImmGetDefaultIMEWnd(IntPtr hWnd);

        [DllImport("imm32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool ImmReleaseContext(IntPtr hWnd, IntPtr hIMC);

        [DllImport("imm32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool ImmNotifyIME(IntPtr hIMC, int dwAction, int dwIndex, int dwValue = 0);

        [DllImport("imm32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool ImmSetCompositionWindow(IntPtr hIMC, ref CompositionForm form);

        [DllImport("imm32.dll", CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool ImmSetCompositionFont(IntPtr hIMC, ref LOGFONT font);

        [DllImport("imm32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool ImmGetCompositionFont(IntPtr hIMC, out LOGFONT font);

        [DllImport("msctf.dll")]
        private static extern int TF_CreateThreadMgr(out ITfThreadMgr threadMgr);

        public static ITfThreadMgr GetTextFrameworkThreadManager()
        {
            if (!textFrameworkThreadMgrInitialized)
            {
                textFrameworkThreadMgrInitialized = true;
                TF_CreateThreadMgr(out textFrameworkThreadMgr);
            }
            return textFrameworkThreadMgr;
        }

        public static bool NotifyIme(IntPtr hIMC)
        {
            return ImmNotifyIME(hIMC, NI_COMPOSITIONSTR, CPS_CANCEL);
        }

        public static bool SetCompositionWindow(HwndSource source, IntPtr hIMC, TextArea textArea)
        {
            if (textArea == null)
            {
                throw new ArgumentNullException("textArea");
            }
            var textViewBounds = textArea.TextView.GetBounds(source);
            Rect characterBounds = textArea.TextView.GetCharacterBounds(textArea.Caret.Position, source);
            var form = new CompositionForm();
            form.dwStyle = 0x0020;
            form.ptCurrentPos.x = (int)Math.Max(characterBounds.Left, textViewBounds.Left);
            form.ptCurrentPos.y = (int)Math.Max(characterBounds.Top, textViewBounds.Top);
            form.rcArea.left = (int)textViewBounds.Left;
            form.rcArea.top = (int)textViewBounds.Top;
            form.rcArea.right = (int)textViewBounds.Right;
            form.rcArea.bottom = (int)textViewBounds.Bottom;
            return ImmSetCompositionWindow(hIMC, ref form);
        }

        public static bool SetCompositionFont(HwndSource source, IntPtr hIMC, TextArea textArea)
        {
            if (textArea == null)
            {
                throw new ArgumentNullException("textArea");
            }
            var lf = new LOGFONT();
            Rect characterBounds = textArea.TextView.GetCharacterBounds(textArea.Caret.Position, source);
            lf.lfFaceName = textArea.FontFamily.Source;
            lf.lfHeight = (int)characterBounds.Height;
            return ImmSetCompositionFont(hIMC, ref lf);
        }

        private static Rect GetBounds(this TextView textView, HwndSource source)
        {
            if (source.RootVisual == null || !source.RootVisual.IsAncestorOf(textView))
            {
                return EMPTY_RECT;
            }
            var displayRect = new Rect(0, 0, textView.ActualWidth, textView.ActualHeight);
            return textView
                .TransformToAncestor(source.RootVisual).TransformBounds(displayRect)
                .TransformToDevice(source.RootVisual);
        }

        private static Rect GetCharacterBounds(this TextView textView, TextViewPosition pos, HwndSource source)
        {
            var vl = textView.GetVisualLine(pos.Line);
            if (vl == null)
            {
                return EMPTY_RECT;
            }

            if (source.RootVisual == null || !source.RootVisual.IsAncestorOf(textView))
            {
                return EMPTY_RECT;
            }
            var line = vl.GetTextLine(pos.VisualColumn, pos.IsAtEndOfLine);
            Rect displayRect;

            if (pos.VisualColumn < vl.VisualLengthWithEndOfLineMarker)
            {
                displayRect = line.GetTextBounds(pos.VisualColumn, 1).First().Rectangle;
                displayRect.Offset(0, vl.GetTextLineVisualYPosition(line, VisualYPosition.LineTop));
            }
            else
            {
                displayRect = new Rect(vl.GetVisualPosition(pos.VisualColumn, VisualYPosition.TextTop),
                    new Size(textView.WideSpaceWidth, textView.DefaultLineHeight));
            }

            displayRect.Offset(-textView.ScrollOffset);
            return textView
                .TransformToAncestor(source.RootVisual).TransformBounds(displayRect)
                .TransformToDevice(source.RootVisual);
        }

        #endregion

        #region Вложенный класс: CompositionForm

        [StructLayout(LayoutKind.Sequential)]
        private struct CompositionForm
        {
            public int dwStyle;

            public POINT ptCurrentPos;

            public RECT rcArea;
        }

        #endregion

        #region Вложенный класс: LOGFONT

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct LOGFONT
        {
            public int lfHeight;

            public readonly int lfWidth;

            public readonly int lfEscapement;

            public readonly int lfOrientation;

            public readonly int lfWeight;

            public readonly byte lfItalic;

            public readonly byte lfUnderline;

            public readonly byte lfStrikeOut;

            public readonly byte lfCharSet;

            public readonly byte lfOutPrecision;

            public readonly byte lfClipPrecision;

            public readonly byte lfQuality;

            public readonly byte lfPitchAndFamily;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string lfFaceName;
        }

        #endregion

        #region Вложенный класс: POINT

        [StructLayout(LayoutKind.Sequential)]
        private struct POINT
        {
            public int x;

            public int y;
        }

        #endregion

        #region Вложенный класс: RECT

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int left;

            public int top;

            public int right;

            public int bottom;
        }

        #endregion
    }
}