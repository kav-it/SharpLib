using System;

using SharpLib.Audio.Wave;

namespace SharpLib.Audio.Utils
{
    internal static class WavePositionExtensions
    {
        #region Методы

        public static TimeSpan GetPositionTimeSpan(this IWavePosition @this)
        {
            var pos = @this.GetPosition() / (@this.OutputWaveFormat.Channels * @this.OutputWaveFormat.BitsPerSample / 8);
            return TimeSpan.FromMilliseconds(pos * 1000.0 / @this.OutputWaveFormat.SampleRate);
        }

        #endregion
    }
}