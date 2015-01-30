using System;
using System.IO;
using System.Runtime.InteropServices;

namespace NAudio.CoreAudioApi.Interfaces
{
    [StructLayout(LayoutKind.Explicit)]
    internal struct PropVariant
    {
        [FieldOffset(0)]
        private short vt;

        [FieldOffset(2)]
        private readonly short wReserved1;

        [FieldOffset(4)]
        private readonly short wReserved2;

        [FieldOffset(6)]
        private readonly short wReserved3;

        [FieldOffset(8)]
        private readonly sbyte cVal;

        [FieldOffset(8)]
        private readonly byte bVal;

        [FieldOffset(8)]
        private readonly short iVal;

        [FieldOffset(8)]
        private readonly ushort uiVal;

        [FieldOffset(8)]
        private readonly int lVal;

        [FieldOffset(8)]
        private readonly uint ulVal;

        [FieldOffset(8)]
        private readonly int intVal;

        [FieldOffset(8)]
        private readonly uint uintVal;

        [FieldOffset(8)]
        private long hVal;

        [FieldOffset(8)]
        private readonly long uhVal;

        [FieldOffset(8)]
        private readonly float fltVal;

        [FieldOffset(8)]
        private readonly double dblVal;

        [FieldOffset(8)]
        private readonly bool boolVal;

        [FieldOffset(8)]
        private readonly int scode;

        [FieldOffset(8)]
        private readonly DateTime date;

        [FieldOffset(8)]
        private readonly System.Runtime.InteropServices.ComTypes.FILETIME filetime;

        [FieldOffset(8)]
        private Blob blobVal;

        [FieldOffset(8)]
        private readonly IntPtr pointerValue;

        public static PropVariant FromLong(long value)
        {
            return new PropVariant
            {
                vt = (short)VarEnum.VT_I8,
                hVal = value
            };
        }

        private byte[] GetBlob()
        {
            var blob = new byte[blobVal.Length];
            Marshal.Copy(blobVal.Data, blob, 0, blob.Length);
            return blob;
        }

        public T[] GetBlobAsArrayOf<T>()
        {
            var blobByteLength = blobVal.Length;
            var singleInstance = (T)Activator.CreateInstance(typeof(T));
            var structSize = Marshal.SizeOf(singleInstance);
            if (blobByteLength % structSize != 0)
            {
                throw new InvalidDataException(String.Format("Blob size {0} not a multiple of struct size {1}", blobByteLength, structSize));
            }
            var items = blobByteLength / structSize;
            var array = new T[items];
            for (int n = 0; n < items; n++)
            {
                array[n] = (T)Activator.CreateInstance(typeof(T));
                Marshal.PtrToStructure(new IntPtr((long)blobVal.Data + n * structSize), array[n]);
            }
            return array;
        }

        public VarEnum DataType
        {
            get { return (VarEnum)vt; }
        }

        public object Value
        {
            get
            {
                VarEnum ve = DataType;
                switch (ve)
                {
                    case VarEnum.VT_I1:
                        return bVal;
                    case VarEnum.VT_I2:
                        return iVal;
                    case VarEnum.VT_I4:
                        return lVal;
                    case VarEnum.VT_I8:
                        return hVal;
                    case VarEnum.VT_INT:
                        return iVal;
                    case VarEnum.VT_UI4:
                        return ulVal;
                    case VarEnum.VT_UI8:
                        return uhVal;
                    case VarEnum.VT_LPWSTR:
                        return Marshal.PtrToStringUni(pointerValue);
                    case VarEnum.VT_BLOB:
                    case VarEnum.VT_VECTOR | VarEnum.VT_UI1:
                        return GetBlob();
                    case VarEnum.VT_CLSID:
                        return (Guid)Marshal.PtrToStructure(pointerValue, typeof(Guid));
                }
                throw new NotImplementedException("PropVariant " + ve);
            }
        }

        public void Clear()
        {
            PropVariantClear(ref this);
        }

        [DllImport("ole32.dll")]
        private static extern int PropVariantClear(ref PropVariant pvar);
    }
}