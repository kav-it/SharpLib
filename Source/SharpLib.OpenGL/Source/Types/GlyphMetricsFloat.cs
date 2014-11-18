using System.Runtime.InteropServices;

namespace SharpLib.OpenGL
{
    [StructLayout(LayoutKind.Sequential)]
    public struct GLYPHMETRICSFLOAT
    {
        public float gmfBlackBoxX;

        public float gmfBlackBoxY;

        public POINTFLOAT gmfptGlyphOrigin;

        public float gmfCellIncX;

        public float gmfCellIncY;
    };
}