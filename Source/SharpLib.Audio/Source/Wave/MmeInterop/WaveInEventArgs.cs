using System;

namespace SharpLib.Audio.Wave
{
    internal class WaveInEventArgs : EventArgs
    {
        #region Поля

        private readonly byte[] buffer;

        private readonly int bytes;

        #endregion

        #region Свойства

        public byte[] Buffer
        {
            get { return buffer; }
        }

        public int BytesRecorded
        {
            get { return bytes; }
        }

        #endregion

        #region Конструктор

        public WaveInEventArgs(byte[] buffer, int bytes)
        {
            this.buffer = buffer;
            this.bytes = bytes;
        }

        #endregion
    }
}