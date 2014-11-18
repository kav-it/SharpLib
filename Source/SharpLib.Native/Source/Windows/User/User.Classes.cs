using System;
using System.Runtime.InteropServices;

namespace SharpLib.Native.Windows
{
    public partial class NativeMethods
    {
        #region Перечисления

        public enum MapType : uint
        {
            MAPVK_VK_TO_VSC = 0x0,

            MAPVK_VSC_TO_VK = 0x1,

            MAPVK_VK_TO_CHAR = 0x2,

            MAPVK_VSC_TO_VK_EX = 0x3,
        }

        public enum SystemMenu
        {
            Size = 0xF000,

            Close = 0xF060,

            Restore = 0xF120,

            Minimize = 0xF020,

            Maximize = 0xF030,
        }

        #endregion

        #region Вложенный класс: POINT

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;

            public int Y;

            public POINT(int x, int y)
            {
                X = x;
                Y = y;
            }

            public override String ToString()
            {
                String text = String.Format("X: {0}, Y: {1}", X, Y);

                return text;
            }
        }

        #endregion

        #region Вложенный класс: RECT

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;

            public int Top;

            public int Right;

            public int Bottom;

            public int Height
            {
                get { return Bottom - Top; }
            }

            public int Width
            {
                get { return Right - Left; }
            }

            public POINT Center
            {
                get
                {
                    POINT p = new POINT();

                    p.X = Left + (int)Math.Round((double)Width / 2);
                    p.Y = Top + (int)Math.Round((double)Height / 2);

                    return p;
                }
                set
                {
                    POINT p = new POINT(value.X, value.Y);

                    Left = p.X - (int)Math.Round((double)Width / 2);
                    Top = p.Y - (int)Math.Round((double)Height / 2);
                }
            }

            public RECT(int left, int top, int width, int height)
            {
                Left = left;
                Top = top;
                Right = left + width;
                Bottom = top + height;
            }

            public override String ToString()
            {
                String text = String.Format("X: {0}, Y: {1}, Width: {2}, Height: {3}", Left, Top, Width, Height);

                return text;
            }
        }

        #endregion
    }
}