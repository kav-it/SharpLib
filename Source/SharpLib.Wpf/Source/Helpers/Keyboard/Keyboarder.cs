using System;
using System.Runtime.InteropServices;
using System.Windows.Input;

namespace SharpLib.Wpf
{
    public static class Keyboarder
    {
        [DllImport("user32.dll")]
        private static extern bool GetKeyboardState(Byte[] lpKeyState);

        #region Константы

        private const UInt16 VK_INSERT = 0x2D;

        #endregion

        #region Поля

        private static readonly byte[] _keybordKeys;

        #endregion

        #region Свойства

        public static bool IsControl
        {
            get
            {
                bool result = Keyboard.IsKeyDown(Key.LeftCtrl) | Keyboard.IsKeyDown(Key.RightCtrl);

                return result;
            }
        }

        public static bool IsShift
        {
            get
            {
                bool result = Keyboard.IsKeyDown(Key.LeftShift) | Keyboard.IsKeyDown(Key.RightShift);

                return result;
            }
        }

        public static bool IsAlt
        {
            get
            {
                bool result = Keyboard.IsKeyDown(Key.LeftAlt) | Keyboard.IsKeyDown(Key.RightAlt);

                return result;
            }
        }

        public static bool IsNumLock
        {
            get { return Console.NumberLock; }
        }

        public static bool IsCapsLock
        {
            get { return Console.CapsLock; }
        }

        public static bool IsInsert
        {
            get { return GetKeyState(VK_INSERT); }
        }

        public static bool IsWin
        {
            get { return Keyboard.IsKeyDown(Key.LWin) | Keyboard.IsKeyDown(Key.RWin); }
        }

        #endregion

        #region Конструктор

        static Keyboarder()
        {
            _keybordKeys = new Byte[255];
        }

        #endregion

        #region Методы

        private static bool GetKeyState(UInt16 keyCode)
        {
            GetKeyboardState(_keybordKeys);

            bool result = (_keybordKeys[keyCode] & 1) == 1;

            return result;
        }

        public static ModifierKeys GetKeyModifies()
        {
            var result = ModifierKeys.None;

            if (IsControl)
            {
                result |= ModifierKeys.Control;
            }
            if (IsShift)
            {
                result |= ModifierKeys.Shift;
            }
            if (IsAlt)
            {
                result |= ModifierKeys.Alt;
            }
            if (IsWin)
            {
                result |= ModifierKeys.Windows;
            }

            return result;
        }

        #endregion
    }
}