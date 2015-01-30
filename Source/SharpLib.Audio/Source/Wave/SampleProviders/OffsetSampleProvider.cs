using System;

namespace NAudio.Wave.SampleProviders
{
    internal class OffsetSampleProvider : ISampleProvider
    {
        #region Поля

        private readonly ISampleProvider sourceProvider;

        private int delayBySamples;

        private int leadOutSamples;

        private int phase;

        private int phasePos;

        private int skipOverSamples;

        private int takeSamples;

        #endregion

        #region Свойства

        public int DelayBySamples
        {
            get { return delayBySamples; }
            set
            {
                if (phase != 0)
                {
                    throw new InvalidOperationException("Can't set DelayBySamples after calling Read");
                }
                if (value % WaveFormat.Channels != 0)
                {
                    throw new ArgumentException("DelayBySamples must be a multiple of WaveFormat.Channels");
                }
                delayBySamples = value;
            }
        }

        public TimeSpan DelayBy
        {
            get { return SamplesToTimeSpan(delayBySamples); }
            set { delayBySamples = TimeSpanToSamples(value); }
        }

        public int SkipOverSamples
        {
            get { return skipOverSamples; }
            set
            {
                if (phase != 0)
                {
                    throw new InvalidOperationException("Can't set SkipOverSamples after calling Read");
                }
                if (value % WaveFormat.Channels != 0)
                {
                    throw new ArgumentException("SkipOverSamples must be a multiple of WaveFormat.Channels");
                }
                skipOverSamples = value;
            }
        }

        public TimeSpan SkipOver
        {
            get { return SamplesToTimeSpan(skipOverSamples); }
            set { skipOverSamples = TimeSpanToSamples(value); }
        }

        public int TakeSamples
        {
            get { return takeSamples; }
            set
            {
                if (phase != 0)
                {
                    throw new InvalidOperationException("Can't set TakeSamples after calling Read");
                }
                if (value % WaveFormat.Channels != 0)
                {
                    throw new ArgumentException("TakeSamples must be a multiple of WaveFormat.Channels");
                }
                takeSamples = value;
            }
        }

        public TimeSpan Take
        {
            get { return SamplesToTimeSpan(takeSamples); }
            set { takeSamples = TimeSpanToSamples(value); }
        }

        public int LeadOutSamples
        {
            get { return leadOutSamples; }
            set
            {
                if (phase != 0)
                {
                    throw new InvalidOperationException("Can't set LeadOutSamples after calling Read");
                }
                if (value % WaveFormat.Channels != 0)
                {
                    throw new ArgumentException("LeadOutSamples must be a multiple of WaveFormat.Channels");
                }
                leadOutSamples = value;
            }
        }

        public TimeSpan LeadOut
        {
            get { return SamplesToTimeSpan(leadOutSamples); }
            set { leadOutSamples = TimeSpanToSamples(value); }
        }

        public WaveFormat WaveFormat
        {
            get { return sourceProvider.WaveFormat; }
        }

        #endregion

        #region Конструктор

        public OffsetSampleProvider(ISampleProvider sourceProvider)
        {
            this.sourceProvider = sourceProvider;
        }

        #endregion

        #region Методы

        private int TimeSpanToSamples(TimeSpan time)
        {
            var samples = (int)(time.TotalSeconds * WaveFormat.SampleRate) * WaveFormat.Channels;
            return samples;
        }

        private TimeSpan SamplesToTimeSpan(int samples)
        {
            return TimeSpan.FromSeconds((samples / WaveFormat.Channels) / (double)WaveFormat.SampleRate);
        }

        public int Read(float[] buffer, int offset, int count)
        {
            int samplesRead = 0;

            if (phase == 0)
            {
                phase++;
            }

            if (phase == 1)
            {
                int delaySamples = Math.Min(count, DelayBySamples - phasePos);
                for (int n = 0; n < delaySamples; n++)
                {
                    buffer[offset + n] = 0;
                }
                phasePos += delaySamples;
                samplesRead += delaySamples;
                if (phasePos >= DelayBySamples)
                {
                    phase++;
                    phasePos = 0;
                }
            }

            if (phase == 2)
            {
                if (SkipOverSamples > 0)
                {
                    var skipBuffer = new float[WaveFormat.SampleRate * WaveFormat.Channels];

                    int samplesSkipped = 0;
                    while (samplesSkipped < SkipOverSamples)
                    {
                        int samplesRequired = Math.Min(SkipOverSamples - samplesSkipped, skipBuffer.Length);
                        var read = sourceProvider.Read(skipBuffer, 0, samplesRequired);
                        if (read == 0)
                        {
                            break;
                        }
                        samplesSkipped += read;
                    }
                }
                phase++;
                phasePos = 0;
            }

            if (phase == 3)
            {
                int samplesRequired = count - samplesRead;
                if (TakeSamples != 0)
                {
                    samplesRequired = Math.Min(samplesRequired, TakeSamples - phasePos);
                }
                int read = sourceProvider.Read(buffer, offset + samplesRead, samplesRequired);
                phasePos += read;
                samplesRead += read;
                if (read < samplesRequired)
                {
                    phase++;
                    phasePos = 0;
                }
            }

            if (phase == 4)
            {
                int samplesRequired = Math.Min(count - samplesRead, LeadOutSamples - phasePos);
                for (int n = 0; n < samplesRequired; n++)
                {
                    buffer[offset + samplesRead + n] = 0;
                }
                phasePos += samplesRequired;
                samplesRead += samplesRequired;
                if (phasePos >= LeadOutSamples)
                {
                    phase++;
                    phasePos = 0;
                }
            }

            return samplesRead;
        }

        #endregion
    }
}