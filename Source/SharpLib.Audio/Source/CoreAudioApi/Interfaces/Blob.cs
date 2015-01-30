using System;

namespace SharpLib.Audio.CoreAudioApi.Interfaces
{
    internal struct Blob
    {
        #region Поля

        public IntPtr Data;

        public int Length;

        #endregion

        #region Методы

        private void FixCS0649()
        {
            Length = 0;
            Data = IntPtr.Zero;
        }

        #endregion
    }
}