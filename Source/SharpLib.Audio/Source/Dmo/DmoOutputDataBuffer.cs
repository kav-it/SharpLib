using System;
using System.Runtime.InteropServices;

namespace NAudio.Dmo
{
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    internal struct DmoOutputDataBuffer : IDisposable
    {
        [MarshalAs(UnmanagedType.Interface)]
        private IMediaBuffer pBuffer;

        private DmoOutputDataBufferFlags dwStatus;

        private long rtTimestamp;

        private long referenceTimeDuration;

        public DmoOutputDataBuffer(int maxBufferSize)
        {
            pBuffer = new MediaBuffer(maxBufferSize);
            dwStatus = DmoOutputDataBufferFlags.None;
            rtTimestamp = 0;
            referenceTimeDuration = 0;
        }

        public void Dispose()
        {
            if (pBuffer != null)
            {
                ((MediaBuffer)pBuffer).Dispose();
                pBuffer = null;
                GC.SuppressFinalize(this);
            }
        }

        public IMediaBuffer MediaBuffer
        {
            get { return pBuffer; }
            internal set { pBuffer = value; }
        }

        public int Length
        {
            get { return ((MediaBuffer)pBuffer).Length; }
        }

        public DmoOutputDataBufferFlags StatusFlags
        {
            get { return dwStatus; }
            internal set { dwStatus = value; }
        }

        public long Timestamp
        {
            get { return rtTimestamp; }
            internal set { rtTimestamp = value; }
        }

        public long Duration
        {
            get { return referenceTimeDuration; }
            internal set { referenceTimeDuration = value; }
        }

        public void RetrieveData(byte[] data, int offset)
        {
            ((MediaBuffer)pBuffer).RetrieveData(data, offset);
        }

        public bool MoreDataAvailable
        {
            get { return (StatusFlags & DmoOutputDataBufferFlags.Incomplete) == DmoOutputDataBufferFlags.Incomplete; }
        }
    }
}