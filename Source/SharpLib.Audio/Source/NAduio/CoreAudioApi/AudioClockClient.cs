using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

using SharpLib.Audio.CoreAudioApi.Interfaces;

namespace SharpLib.Audio.CoreAudioApi
{
    internal class AudioClockClient : IDisposable
    {
        #region Поля

        private IAudioClock audioClockClientInterface;

        #endregion

        #region Свойства

        public int Characteristics
        {
            get
            {
                uint characteristics;
                Marshal.ThrowExceptionForHR(audioClockClientInterface.GetCharacteristics(out characteristics));
                return (int)characteristics;
            }
        }

        public ulong Frequency
        {
            get
            {
                ulong freq;
                Marshal.ThrowExceptionForHR(audioClockClientInterface.GetFrequency(out freq));
                return freq;
            }
        }

        public ulong AdjustedPosition
        {
            get
            {
                var byteLatency = (TimeSpan.TicksPerSecond / Frequency);

                ulong pos, qpos;
                int cnt = 0;
                while (!GetPosition(out pos, out qpos))
                {
                    if (++cnt == 5)
                    {
                        break;
                    }
                }

                if (Stopwatch.IsHighResolution)
                {
                    var qposNow = (ulong)((Stopwatch.GetTimestamp() * 10000000M) / Stopwatch.Frequency);

                    var qposDiff = (qposNow - qpos) / 100;

                    var bytes = qposDiff / byteLatency;

                    pos += bytes;
                }
                return pos;
            }
        }

        public bool CanAdjustPosition
        {
            get { return Stopwatch.IsHighResolution; }
        }

        #endregion

        #region Конструктор

        internal AudioClockClient(IAudioClock audioClockClientInterface)
        {
            this.audioClockClientInterface = audioClockClientInterface;
        }

        #endregion

        #region Методы

        public bool GetPosition(out ulong position, out ulong qpcPosition)
        {
            var hr = audioClockClientInterface.GetPosition(out position, out qpcPosition);
            if (hr == -1)
            {
                return false;
            }
            Marshal.ThrowExceptionForHR(hr);
            return true;
        }

        public void Dispose()
        {
            if (audioClockClientInterface != null)
            {
                Marshal.ReleaseComObject(audioClockClientInterface);
                audioClockClientInterface = null;
                GC.SuppressFinalize(this);
            }
        }

        #endregion
    }
}