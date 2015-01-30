using System;

namespace SharpLib.Audio.Wave.SampleProviders
{
    internal class SignalGenerator : ISampleProvider
    {
        #region Константы

        private const double TwoPi = 2 * Math.PI;

        #endregion

        #region Поля

        private readonly double[] pinkNoiseBuffer = new double[7];

        private readonly Random random = new Random();

        private readonly WaveFormat waveFormat;

        private int nSample;

        private double phi;

        #endregion

        #region Свойства

        public WaveFormat WaveFormat
        {
            get { return waveFormat; }
        }

        public double Frequency { get; set; }

        public double FrequencyLog
        {
            get { return Math.Log(Frequency); }
        }

        public double FrequencyEnd { get; set; }

        public double FrequencyEndLog
        {
            get { return Math.Log(FrequencyEnd); }
        }

        public double Gain { get; set; }

        public bool[] PhaseReverse { get; private set; }

        public SignalGeneratorType Type { get; set; }

        public double SweepLengthSecs { get; set; }

        #endregion

        #region Конструктор

        public SignalGenerator()
            : this(44100, 2)
        {
        }

        public SignalGenerator(int sampleRate, int channel)
        {
            phi = 0;
            waveFormat = WaveFormat.CreateIeeeFloatWaveFormat(sampleRate, channel);

            Type = SignalGeneratorType.Sin;
            Frequency = 440.0;
            Gain = 1;
            PhaseReverse = new bool[channel];
            SweepLengthSecs = 2;
        }

        #endregion

        #region Методы

        public int Read(float[] buffer, int offset, int count)
        {
            int outIndex = offset;

            double multiple;
            double sampleValue;
            double sampleSaw;

            for (int sampleCount = 0; sampleCount < count / waveFormat.Channels; sampleCount++)
            {
                switch (Type)
                {
                    case SignalGeneratorType.Sin:

                        multiple = TwoPi * Frequency / waveFormat.SampleRate;
                        sampleValue = Gain * Math.Sin(nSample * multiple);

                        nSample++;

                        break;

                    case SignalGeneratorType.Square:

                        multiple = 2 * Frequency / waveFormat.SampleRate;
                        sampleSaw = ((nSample * multiple) % 2) - 1;
                        sampleValue = sampleSaw > 0 ? Gain : -Gain;

                        nSample++;
                        break;

                    case SignalGeneratorType.Triangle:

                        multiple = 2 * Frequency / waveFormat.SampleRate;
                        sampleSaw = ((nSample * multiple) % 2);
                        sampleValue = 2 * sampleSaw;
                        if (sampleValue > 1)
                        {
                            sampleValue = 2 - sampleValue;
                        }
                        if (sampleValue < -1)
                        {
                            sampleValue = -2 - sampleValue;
                        }

                        sampleValue *= Gain;

                        nSample++;
                        break;

                    case SignalGeneratorType.SawTooth:

                        multiple = 2 * Frequency / waveFormat.SampleRate;
                        sampleSaw = ((nSample * multiple) % 2) - 1;
                        sampleValue = Gain * sampleSaw;

                        nSample++;
                        break;

                    case SignalGeneratorType.White:

                        sampleValue = (Gain * NextRandomTwo());
                        break;

                    case SignalGeneratorType.Pink:

                        double white = NextRandomTwo();
                        pinkNoiseBuffer[0] = 0.99886 * pinkNoiseBuffer[0] + white * 0.0555179;
                        pinkNoiseBuffer[1] = 0.99332 * pinkNoiseBuffer[1] + white * 0.0750759;
                        pinkNoiseBuffer[2] = 0.96900 * pinkNoiseBuffer[2] + white * 0.1538520;
                        pinkNoiseBuffer[3] = 0.86650 * pinkNoiseBuffer[3] + white * 0.3104856;
                        pinkNoiseBuffer[4] = 0.55000 * pinkNoiseBuffer[4] + white * 0.5329522;
                        pinkNoiseBuffer[5] = -0.7616 * pinkNoiseBuffer[5] - white * 0.0168980;
                        double pink = pinkNoiseBuffer[0] + pinkNoiseBuffer[1] + pinkNoiseBuffer[2] + pinkNoiseBuffer[3] + pinkNoiseBuffer[4] + pinkNoiseBuffer[5] + pinkNoiseBuffer[6] + white * 0.5362;
                        pinkNoiseBuffer[6] = white * 0.115926;
                        sampleValue = (Gain * (pink / 5));
                        break;

                    case SignalGeneratorType.Sweep:

                        double f = Math.Exp(FrequencyLog + (nSample * (FrequencyEndLog - FrequencyLog)) / (SweepLengthSecs * waveFormat.SampleRate));

                        multiple = TwoPi * f / waveFormat.SampleRate;
                        phi += multiple;
                        sampleValue = Gain * (Math.Sin(phi));
                        nSample++;
                        if (nSample > SweepLengthSecs * waveFormat.SampleRate)
                        {
                            nSample = 0;
                            phi = 0;
                        }
                        break;

                    default:
                        sampleValue = 0.0;
                        break;
                }

                for (int i = 0; i < waveFormat.Channels; i++)
                {
                    if (PhaseReverse[i])
                    {
                        buffer[outIndex++] = (float)-sampleValue;
                    }
                    else
                    {
                        buffer[outIndex++] = (float)sampleValue;
                    }
                }
            }
            return count;
        }

        private double NextRandomTwo()
        {
            return 2 * random.NextDouble() - 1;
        }

        #endregion
    }

    internal enum SignalGeneratorType
    {
        Pink,

        White,

        Sweep,

        Sin,

        Square,

        Triangle,

        SawTooth,
    }
}