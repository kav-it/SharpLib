using System;
using System.Diagnostics;

using NAudio.Dmo;
using NAudio.Wave;

namespace NAudio.FileFormats.Mp3
{
    internal class DmoMp3FrameDecompressor : IMp3FrameDecompressor
    {
        #region Поля

        private readonly WaveFormat pcmFormat;

        private MediaBuffer inputMediaBuffer;

        private WindowsMediaMp3Decoder mp3Decoder;

        private DmoOutputDataBuffer outputBuffer;

        private bool reposition;

        #endregion

        #region Свойства

        public WaveFormat OutputFormat
        {
            get { return pcmFormat; }
        }

        #endregion

        #region Конструктор

        public DmoMp3FrameDecompressor(WaveFormat sourceFormat)
        {
            mp3Decoder = new WindowsMediaMp3Decoder();
            if (!mp3Decoder.MediaObject.SupportsInputWaveFormat(0, sourceFormat))
            {
                throw new ArgumentException("Unsupported input format");
            }
            mp3Decoder.MediaObject.SetInputWaveFormat(0, sourceFormat);
            pcmFormat = new WaveFormat(sourceFormat.SampleRate, sourceFormat.Channels);
            if (!mp3Decoder.MediaObject.SupportsOutputWaveFormat(0, pcmFormat))
            {
                throw new ArgumentException(String.Format("Unsupported output format {0}", pcmFormat));
            }
            mp3Decoder.MediaObject.SetOutputWaveFormat(0, pcmFormat);

            inputMediaBuffer = new MediaBuffer(sourceFormat.AverageBytesPerSecond);
            outputBuffer = new DmoOutputDataBuffer(pcmFormat.AverageBytesPerSecond);
        }

        #endregion

        #region Методы

        public int DecompressFrame(Mp3Frame frame, byte[] dest, int destOffset)
        {
            inputMediaBuffer.LoadData(frame.RawData, frame.FrameLength);

            if (reposition)
            {
                mp3Decoder.MediaObject.Flush();
                reposition = false;
            }

            mp3Decoder.MediaObject.ProcessInput(0, inputMediaBuffer, DmoInputDataBufferFlags.None, 0, 0);

            outputBuffer.MediaBuffer.SetLength(0);
            outputBuffer.StatusFlags = DmoOutputDataBufferFlags.None;

            mp3Decoder.MediaObject.ProcessOutput(DmoProcessOutputFlags.None, 1, new[] { outputBuffer });

            if (outputBuffer.Length == 0)
            {
                Debug.WriteLine("ResamplerDmoStream.Read: No output data available");
                return 0;
            }

            outputBuffer.RetrieveData(dest, destOffset);
            Debug.Assert(!outputBuffer.MoreDataAvailable, "have not implemented more data available yet");

            return outputBuffer.Length;
        }

        public void Reset()
        {
            reposition = true;
        }

        public void Dispose()
        {
            if (inputMediaBuffer != null)
            {
                inputMediaBuffer.Dispose();
                inputMediaBuffer = null;
            }
            outputBuffer.Dispose();
            if (mp3Decoder != null)
            {
                mp3Decoder.Dispose();
                mp3Decoder = null;
            }
        }

        #endregion
    }
}