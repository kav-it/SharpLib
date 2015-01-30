using System;
using System.Runtime.InteropServices;

using NAudio.Utils;

namespace NAudio.Dmo
{
    internal class MediaBuffer : IMediaBuffer, IDisposable
    {
        #region Поля

        private readonly int maxLength;

        private IntPtr buffer;

        private int length;

        #endregion

        #region Свойства

        public int Length
        {
            get { return length; }
            set
            {
                if (length > maxLength)
                {
                    throw new ArgumentException("Cannot be greater than maximum buffer size");
                }
                length = value;
            }
        }

        #endregion

        #region Конструктор

        public MediaBuffer(int maxLength)
        {
            buffer = Marshal.AllocCoTaskMem(maxLength);
            this.maxLength = maxLength;
        }

        ~MediaBuffer()
        {
            Dispose();
        }

        #endregion

        #region Методы

        public void Dispose()
        {
            if (buffer != IntPtr.Zero)
            {
                Marshal.FreeCoTaskMem(buffer);
                buffer = IntPtr.Zero;
                GC.SuppressFinalize(this);
            }
        }

        int IMediaBuffer.SetLength(int length)
        {
            if (length > maxLength)
            {
                return HResult.E_INVALIDARG;
            }
            this.length = length;
            return HResult.S_OK;
        }

        int IMediaBuffer.GetMaxLength(out int maxLength)
        {
            maxLength = this.maxLength;
            return HResult.S_OK;
        }

        int IMediaBuffer.GetBufferAndLength(IntPtr bufferPointerPointer, IntPtr validDataLengthPointer)
        {
            if (bufferPointerPointer != IntPtr.Zero)
            {
                Marshal.WriteIntPtr(bufferPointerPointer, buffer);
            }
            if (validDataLengthPointer != IntPtr.Zero)
            {
                Marshal.WriteInt32(validDataLengthPointer, length);
            }

            return HResult.S_OK;
        }

        public void LoadData(byte[] data, int bytes)
        {
            Length = bytes;
            Marshal.Copy(data, 0, buffer, bytes);
        }

        public void RetrieveData(byte[] data, int offset)
        {
            Marshal.Copy(buffer, data, offset, Length);
        }

        #endregion
    }
}