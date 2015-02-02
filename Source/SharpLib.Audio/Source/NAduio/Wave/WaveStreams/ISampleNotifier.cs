using System;

namespace SharpLib.Audio.Wave
{
    internal interface ISampleNotifier
    {
        #region События

        event EventHandler<SampleEventArgs> Sample;

        #endregion
    }

    internal class SampleEventArgs : EventArgs
    {
        #region Свойства

        public float Left { get; set; }

        public float Right { get; set; }

        #endregion

        #region Конструктор

        public SampleEventArgs(float left, float right)
        {
            Left = left;
            Right = right;
        }

        #endregion
    }
}