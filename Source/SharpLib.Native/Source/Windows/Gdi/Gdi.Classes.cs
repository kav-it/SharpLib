using System;
using System.Runtime.InteropServices;

namespace SharpLib.Native.Windows
{
    public partial class NativeMethods
    {
        #region Вложенный класс: PIXELFORMATDESCRIPTOR

        [StructLayout(LayoutKind.Sequential)]
        public struct PIXELFORMATDESCRIPTOR
        {
            public Int16 nSize;

            public Int16 nVersion;

            public Int32 dwFlags;

            public Byte iPixelType;

            public Byte cColorBits;

            public Byte cRedBits;

            public Byte cRedShift;

            public Byte cGreenBits;

            public Byte cGreenShift;

            public Byte cBlueBits;

            public Byte cBlueShift;

            public Byte cAlphaBits;

            public Byte cAlphaShift;

            public Byte cAccumBits;

            public Byte cAccumRedBits;

            public Byte cAccumGreenBits;

            public Byte cAccumBlueBits;

            public Byte cAccumAlphaBits;

            public Byte cDepthBits;

            public Byte cStencilBits;

            public Byte cAuxBuffers;

            public Byte iLayerType;

            public Byte bReserved;

            public Int32 dwLayerMask;

            public Int32 dwVisibleMask;

            public Int32 dwDamageMask;
        };

        #endregion
    }
}