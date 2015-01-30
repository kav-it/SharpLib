using System;
using System.Collections.Generic;

using NAudio.Utils;

namespace NAudio.Wave.SampleProviders
{
    internal class MultiplexingSampleProvider : ISampleProvider
    {
        #region Поля

        private readonly int inputChannelCount;

        private readonly IList<ISampleProvider> inputs;

        private readonly List<int> mappings;

        private readonly int outputChannelCount;

        private readonly WaveFormat waveFormat;

        private float[] inputBuffer;

        #endregion

        #region Свойства

        public WaveFormat WaveFormat
        {
            get { return waveFormat; }
        }

        public int InputChannelCount
        {
            get { return inputChannelCount; }
        }

        public int OutputChannelCount
        {
            get { return outputChannelCount; }
        }

        #endregion

        #region Конструктор

        public MultiplexingSampleProvider(IEnumerable<ISampleProvider> inputs, int numberOfOutputChannels)
        {
            this.inputs = new List<ISampleProvider>(inputs);
            outputChannelCount = numberOfOutputChannels;

            if (this.inputs.Count == 0)
            {
                throw new ArgumentException("You must provide at least one input");
            }
            if (numberOfOutputChannels < 1)
            {
                throw new ArgumentException("You must provide at least one output");
            }
            foreach (var input in this.inputs)
            {
                if (waveFormat == null)
                {
                    if (input.WaveFormat.Encoding != WaveFormatEncoding.IeeeFloat)
                    {
                        throw new ArgumentException("Only 32 bit float is supported");
                    }
                    waveFormat = WaveFormat.CreateIeeeFloatWaveFormat(input.WaveFormat.SampleRate, numberOfOutputChannels);
                }
                else
                {
                    if (input.WaveFormat.BitsPerSample != waveFormat.BitsPerSample)
                    {
                        throw new ArgumentException("All inputs must have the same bit depth");
                    }
                    if (input.WaveFormat.SampleRate != waveFormat.SampleRate)
                    {
                        throw new ArgumentException("All inputs must have the same sample rate");
                    }
                }
                inputChannelCount += input.WaveFormat.Channels;
            }

            mappings = new List<int>();
            for (int n = 0; n < outputChannelCount; n++)
            {
                mappings.Add(n % inputChannelCount);
            }
        }

        #endregion

        #region Методы

        public int Read(float[] buffer, int offset, int count)
        {
            int sampleFramesRequested = count / outputChannelCount;
            int inputOffset = 0;
            int sampleFramesRead = 0;

            foreach (var input in inputs)
            {
                int samplesRequired = sampleFramesRequested * input.WaveFormat.Channels;
                inputBuffer = BufferHelpers.Ensure(inputBuffer, samplesRequired);
                int samplesRead = input.Read(inputBuffer, 0, samplesRequired);
                sampleFramesRead = Math.Max(sampleFramesRead, samplesRead / input.WaveFormat.Channels);

                for (int n = 0; n < input.WaveFormat.Channels; n++)
                {
                    int inputIndex = inputOffset + n;
                    for (int outputIndex = 0; outputIndex < outputChannelCount; outputIndex++)
                    {
                        if (mappings[outputIndex] == inputIndex)
                        {
                            int inputBufferOffset = n;
                            int outputBufferOffset = offset + outputIndex;
                            int sample = 0;
                            while (sample < sampleFramesRequested && inputBufferOffset < samplesRead)
                            {
                                buffer[outputBufferOffset] = inputBuffer[inputBufferOffset];
                                outputBufferOffset += outputChannelCount;
                                inputBufferOffset += input.WaveFormat.Channels;
                                sample++;
                            }

                            while (sample < sampleFramesRequested)
                            {
                                buffer[outputBufferOffset] = 0;
                                outputBufferOffset += outputChannelCount;
                                sample++;
                            }
                        }
                    }
                }
                inputOffset += input.WaveFormat.Channels;
            }

            return sampleFramesRead * outputChannelCount;
        }

        public void ConnectInputToOutput(int inputChannel, int outputChannel)
        {
            if (inputChannel < 0 || inputChannel >= InputChannelCount)
            {
                throw new ArgumentException("Invalid input channel");
            }
            if (outputChannel < 0 || outputChannel >= OutputChannelCount)
            {
                throw new ArgumentException("Invalid output channel");
            }
            mappings[outputChannel] = inputChannel;
        }

        #endregion
    }
}