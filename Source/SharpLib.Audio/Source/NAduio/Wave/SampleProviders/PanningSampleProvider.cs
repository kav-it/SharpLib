using System;

using SharpLib.Audio.Utils;

namespace SharpLib.Audio.Wave.SampleProviders
{
    internal class PanningSampleProvider : ISampleProvider
    {
        #region Поля

        private readonly ISampleProvider source;

        private readonly WaveFormat waveFormat;

        private float leftMultiplier;

        private float pan;

        private IPanStrategy panStrategy;

        private float rightMultiplier;

        private float[] sourceBuffer;

        #endregion

        #region Свойства

        public float Pan
        {
            get { return pan; }
            set
            {
                if (value < -1.0f || value > 1.0f)
                {
                    throw new ArgumentOutOfRangeException("value", "Pan must be in the range -1 to 1");
                }
                pan = value;
                UpdateMultipliers();
            }
        }

        public IPanStrategy PanStrategy
        {
            get { return panStrategy; }
            set
            {
                panStrategy = value;
                UpdateMultipliers();
            }
        }

        public WaveFormat WaveFormat
        {
            get { return waveFormat; }
        }

        #endregion

        #region Конструктор

        public PanningSampleProvider(ISampleProvider source)
        {
            if (source.WaveFormat.Channels != 1)
            {
                throw new ArgumentException("Source sample provider must be mono");
            }
            this.source = source;
            waveFormat = WaveFormat.CreateIeeeFloatWaveFormat(source.WaveFormat.SampleRate, 2);
            panStrategy = new SinPanStrategy();
        }

        #endregion

        #region Методы

        private void UpdateMultipliers()
        {
            var multipliers = panStrategy.GetMultipliers(Pan);
            leftMultiplier = multipliers.Left;
            rightMultiplier = multipliers.Right;
        }

        public int Read(float[] buffer, int offset, int count)
        {
            int sourceSamplesRequired = count / 2;
            sourceBuffer = BufferHelpers.Ensure(sourceBuffer, sourceSamplesRequired);
            int sourceSamplesRead = source.Read(sourceBuffer, 0, sourceSamplesRequired);
            int outIndex = offset;
            for (int n = 0; n < sourceSamplesRead; n++)
            {
                buffer[outIndex++] = leftMultiplier * sourceBuffer[n];
                buffer[outIndex++] = rightMultiplier * sourceBuffer[n];
            }
            return sourceSamplesRead * 2;
        }

        #endregion
    }

    internal struct StereoSamplePair
    {
        #region Свойства

        public float Left { get; set; }

        public float Right { get; set; }

        #endregion
    }

    internal interface IPanStrategy
    {
        #region Методы

        StereoSamplePair GetMultipliers(float pan);

        #endregion
    }

    internal class StereoBalanceStrategy : IPanStrategy
    {
        #region Методы

        public StereoSamplePair GetMultipliers(float pan)
        {
            float leftChannel = (pan <= 0) ? 1.0f : ((1 - pan) / 2.0f);
            float rightChannel = (pan >= 0) ? 1.0f : ((pan + 1) / 2.0f);

            return new StereoSamplePair
            {
                Left = leftChannel,
                Right = rightChannel
            };
        }

        #endregion
    }

    internal class SquareRootPanStrategy : IPanStrategy
    {
        #region Методы

        public StereoSamplePair GetMultipliers(float pan)
        {
            float normPan = (-pan + 1) / 2;
            float leftChannel = (float)Math.Sqrt(normPan);
            float rightChannel = (float)Math.Sqrt(1 - normPan);

            return new StereoSamplePair
            {
                Left = leftChannel,
                Right = rightChannel
            };
        }

        #endregion
    }

    internal class SinPanStrategy : IPanStrategy
    {
        #region Константы

        private const float HalfPi = (float)Math.PI / 2;

        #endregion

        #region Методы

        public StereoSamplePair GetMultipliers(float pan)
        {
            float normPan = (-pan + 1) / 2;
            float leftChannel = (float)Math.Sin(normPan * HalfPi);
            float rightChannel = (float)Math.Cos(normPan * HalfPi);

            return new StereoSamplePair
            {
                Left = leftChannel,
                Right = rightChannel
            };
        }

        #endregion
    }

    internal class LinearPanStrategy : IPanStrategy
    {
        #region Методы

        public StereoSamplePair GetMultipliers(float pan)
        {
            float normPan = (-pan + 1) / 2;
            float leftChannel = normPan;
            float rightChannel = 1 - normPan;
            return new StereoSamplePair
            {
                Left = leftChannel,
                Right = rightChannel
            };
        }

        #endregion
    }
}