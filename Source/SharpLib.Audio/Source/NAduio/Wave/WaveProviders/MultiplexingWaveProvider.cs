using System;
using System.Collections.Generic;

using SharpLib.Audio.Utils;

namespace SharpLib.Audio.Wave
{
    internal class MultiplexingWaveProvider : IWaveProvider
    {
        #region Поля

        private readonly int bytesPerSample;

        private readonly int inputChannelCount;

        private readonly IList<IWaveProvider> inputs;

        private readonly List<int> mappings;

        private readonly int outputChannelCount;

        private readonly WaveFormat waveFormat;

        private byte[] inputBuffer;

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

        public MultiplexingWaveProvider(IEnumerable<IWaveProvider> inputs, int numberOfOutputChannels)
        {
            this.inputs = new List<IWaveProvider>(inputs);
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
                    if (input.WaveFormat.Encoding == WaveFormatEncoding.Pcm)
                    {
                        waveFormat = new WaveFormat(input.WaveFormat.SampleRate, input.WaveFormat.BitsPerSample, numberOfOutputChannels);
                    }
                    else if (input.WaveFormat.Encoding == WaveFormatEncoding.IeeeFloat)
                    {
                        waveFormat = WaveFormat.CreateIeeeFloatWaveFormat(input.WaveFormat.SampleRate, numberOfOutputChannels);
                    }
                    else
                    {
                        throw new ArgumentException("Only PCM and 32 bit float are supported");
                    }
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
            bytesPerSample = waveFormat.BitsPerSample / 8;

            mappings = new List<int>();
            for (int n = 0; n < outputChannelCount; n++)
            {
                mappings.Add(n % inputChannelCount);
            }
        }

        #endregion

        #region Методы

        public int Read(byte[] buffer, int offset, int count)
        {
            int outputBytesPerFrame = bytesPerSample * outputChannelCount;
            int sampleFramesRequested = count / outputBytesPerFrame;
            int inputOffset = 0;
            int sampleFramesRead = 0;

            foreach (var input in inputs)
            {
                int inputBytesPerFrame = bytesPerSample * input.WaveFormat.Channels;
                int bytesRequired = sampleFramesRequested * inputBytesPerFrame;
                inputBuffer = BufferHelpers.Ensure(inputBuffer, bytesRequired);
                int bytesRead = input.Read(inputBuffer, 0, bytesRequired);
                sampleFramesRead = Math.Max(sampleFramesRead, bytesRead / inputBytesPerFrame);

                for (int n = 0; n < input.WaveFormat.Channels; n++)
                {
                    int inputIndex = inputOffset + n;
                    for (int outputIndex = 0; outputIndex < outputChannelCount; outputIndex++)
                    {
                        if (mappings[outputIndex] == inputIndex)
                        {
                            int inputBufferOffset = n * bytesPerSample;
                            int outputBufferOffset = offset + outputIndex * bytesPerSample;
                            int sample = 0;
                            while (sample < sampleFramesRequested && inputBufferOffset < bytesRead)
                            {
                                Array.Copy(inputBuffer, inputBufferOffset, buffer, outputBufferOffset, bytesPerSample);
                                outputBufferOffset += outputBytesPerFrame;
                                inputBufferOffset += inputBytesPerFrame;
                                sample++;
                            }

                            while (sample < sampleFramesRequested)
                            {
                                Array.Clear(buffer, outputBufferOffset, bytesPerSample);
                                outputBufferOffset += outputBytesPerFrame;
                                sample++;
                            }
                        }
                    }
                }
                inputOffset += input.WaveFormat.Channels;
            }

            return sampleFramesRead * outputBytesPerFrame;
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