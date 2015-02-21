using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

using STATSTG = System.Runtime.InteropServices.ComTypes.STATSTG;

namespace Standard
{
    internal sealed class ManagedIStream : IStream, IDisposable
    {
        #region Константы

        private const int LOCK_EXCLUSIVE = 2;

        private const int STGM_READWRITE = 2;

        private const int STGTY_STREAM = 2;

        #endregion

        #region Поля

        private Stream _source;

        #endregion

        #region Конструктор

        public ManagedIStream(Stream source)
        {
            Verify.IsNotNull(source, "source");
            _source = source;
        }

        #endregion

        #region Методы

        private void _Validate()
        {
            if (null == _source)
            {
                throw new ObjectDisposedException("this");
            }
        }

        [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Standard.HRESULT.ThrowIfFailed(System.String)")]
        [Obsolete("The method is not implemented", true)]
        public void Clone(out IStream ppstm)
        {
            ppstm = null;
            HRESULT.STG_E_INVALIDFUNCTION.ThrowIfFailed("The method is not implemented.");
        }

        public void Commit(int grfCommitFlags)
        {
            _Validate();
            _source.Flush();
        }

        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
        [SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands")]
        public void CopyTo(IStream pstm, long cb, IntPtr pcbRead, IntPtr pcbWritten)
        {
            Verify.IsNotNull(pstm, "pstm");

            _Validate();

            var buffer = new byte[4096];
            long cbWritten = 0;

            while (cbWritten < cb)
            {
                int cbRead = _source.Read(buffer, 0, buffer.Length);
                if (0 == cbRead)
                {
                    break;
                }

                pstm.Write(buffer, cbRead, IntPtr.Zero);
                cbWritten += cbRead;
            }

            if (IntPtr.Zero != pcbRead)
            {
                Marshal.WriteInt64(pcbRead, cbWritten);
            }

            if (IntPtr.Zero != pcbWritten)
            {
                Marshal.WriteInt64(pcbWritten, cbWritten);
            }
        }

        [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Standard.HRESULT.ThrowIfFailed(System.String)"),
         Obsolete("The method is not implemented", true)]
        public void LockRegion(long libOffset, long cb, int dwLockType)
        {
            HRESULT.STG_E_INVALIDFUNCTION.ThrowIfFailed("The method is not implemented.");
        }

        [SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands")]
        public void Read(byte[] pv, int cb, IntPtr pcbRead)
        {
            _Validate();

            int cbRead = _source.Read(pv, 0, cb);

            if (IntPtr.Zero != pcbRead)
            {
                Marshal.WriteInt32(pcbRead, cbRead);
            }
        }

        [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Standard.HRESULT.ThrowIfFailed(System.String)"),
         Obsolete("The method is not implemented", true)]
        public void Revert()
        {
            HRESULT.STG_E_INVALIDFUNCTION.ThrowIfFailed("The method is not implemented.");
        }

        [SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands")]
        public void Seek(long dlibMove, int dwOrigin, IntPtr plibNewPosition)
        {
            _Validate();

            long position = _source.Seek(dlibMove, (SeekOrigin)dwOrigin);

            if (IntPtr.Zero != plibNewPosition)
            {
                Marshal.WriteInt64(plibNewPosition, position);
            }
        }

        public void SetSize(long libNewSize)
        {
            _Validate();
            _source.SetLength(libNewSize);
        }

        public void Stat(out STATSTG pstatstg, int grfStatFlag)
        {
            pstatstg = default(STATSTG);
            _Validate();

            pstatstg.type = STGTY_STREAM;
            pstatstg.cbSize = _source.Length;
            pstatstg.grfMode = STGM_READWRITE;
            pstatstg.grfLocksSupported = LOCK_EXCLUSIVE;
        }

        [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Standard.HRESULT.ThrowIfFailed(System.String)")]
        [Obsolete("The method is not implemented", true)]
        public void UnlockRegion(long libOffset, long cb, int dwLockType)
        {
            HRESULT.STG_E_INVALIDFUNCTION.ThrowIfFailed("The method is not implemented.");
        }

        [SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands")]
        public void Write(byte[] pv, int cb, IntPtr pcbWritten)
        {
            _Validate();

            _source.Write(pv, 0, cb);

            if (IntPtr.Zero != pcbWritten)
            {
                Marshal.WriteInt32(pcbWritten, cb);
            }
        }

        public void Dispose()
        {
            _source = null;
        }

        #endregion
    }
}