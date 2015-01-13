using System;
using System.Runtime.InteropServices;

namespace SharpLib
{
    /// <summary>
    /// Структура предоставляющая доступ к разным видам одного значения int
    /// </summary>
    [StructLayout(LayoutKind.Explicit, Size = 8, Pack = 1)]
    public struct WideInt
    {
        [FieldOffset(0)]
        public byte Value8;

        [FieldOffset(0)]
        public UInt16 Value16;

        [FieldOffset(0)]
        public int Value32;

        [FieldOffset(0)]
        public Int64 Value64;

        [FieldOffset(0)]
        public float ValueFloat;

        [FieldOffset(0)]
        public double ValueDouble;

        [FieldOffset(0)]
        public byte Data0;
        [FieldOffset(1)]
        public byte Data1;
        [FieldOffset(2)]
        public byte Data2;
        [FieldOffset(3)]
        public byte Data3;
        [FieldOffset(4)]
        public byte Data4;
        [FieldOffset(5)]
        public byte Data5;
        [FieldOffset(6)]
        public byte Data6;
        [FieldOffset(7)]
        public byte Data7;

        public WideInt(double valueDouble) : this()
        {
            ValueDouble = valueDouble;
        }

        public WideInt(float valueFloat) : this()
        {
            ValueFloat = valueFloat;
        }

        public WideInt(byte value8) : this()
        {
            Value8 = value8;
        }

        public WideInt(ushort value16) : this()
        {
            Value16 = value16;
        }

        public WideInt(int value32) : this()
        {
            Value32 = value32;
        }

        public WideInt(Int64 value64) : this()
        {
            Value64 = value64;
        }

        public WideInt(byte[] data): this()
        {
            if (data == null)
            {
                return;
            }

            var size = data.Length;

            if (size > 0) { Data0 = data[0]; }
            if (size > 1) { Data1 = data[1]; }
            if (size > 2) { Data2 = data[2]; }
            if (size > 3) { Data3 = data[3]; }
            if (size > 4) { Data4 = data[4]; }
            if (size > 5) { Data5 = data[5]; }
            if (size > 6) { Data6 = data[6]; }
            if (size > 7) { Data7 = data[7]; }
        }
    }
}