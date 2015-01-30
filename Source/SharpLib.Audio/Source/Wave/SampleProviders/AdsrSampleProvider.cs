using System;

using SharpLib.Audio.Dsp;

namespace SharpLib.Audio.Wave.SampleProviders
{
    internal class AdsrSampleProvider : ISampleProvider
    {
        #region Поля

        private readonly EnvelopeGenerator adsr;

        private readonly ISampleProvider source;

        private float attackSeconds;

        private float releaseSeconds;

        #endregion

        #region Свойства

        public float AttackSeconds
        {
            get { return attackSeconds; }
            set
            {
                attackSeconds = value;
                adsr.AttackRate = attackSeconds * WaveFormat.SampleRate;
            }
        }

        public float ReleaseSeconds
        {
            get { return releaseSeconds; }
            set
            {
                releaseSeconds = value;
                adsr.ReleaseRate = releaseSeconds * WaveFormat.SampleRate;
            }
        }

        public WaveFormat WaveFormat
        {
            get { return source.WaveFormat; }
        }

        #endregion

        #region Конструктор

        public AdsrSampleProvider(ISampleProvider source)
        {
            if (source.WaveFormat.Channels > 1)
            {
                throw new ArgumentException("Currently only supports mono inputs");
            }
            this.source = source;
            adsr = new EnvelopeGenerator();
            AttackSeconds = 0.01f;
            adsr.SustainLevel = 1.0f;
            adsr.DecayRate = 0.0f * WaveFormat.SampleRate;
            ReleaseSeconds = 0.3f;
            adsr.Gate(true);
        }

        #endregion

        #region Методы

        public int Read(float[] buffer, int offset, int count)
        {
            if (adsr.State == EnvelopeGenerator.EnvelopeState.Idle)
            {
                return 0;
            }
            var samples = source.Read(buffer, offset, count);
            for (int n = 0; n < samples; n++)
            {
                buffer[offset++] *= adsr.Process();
            }
            return samples;
        }

        public void Stop()
        {
            adsr.Gate(false);
        }

        #endregion
    }
}