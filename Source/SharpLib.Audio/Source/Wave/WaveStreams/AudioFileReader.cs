using System;

using NAudio.Wave.SampleProviders;

namespace NAudio.Wave
{
    internal class AudioFileReader : WaveStream, ISampleProvider
    {
        #region Поля

        private readonly int _destBytesPerSample;

        private readonly long _length;

        private readonly object _lockObject;

        private readonly SampleChannel _sampleChannel;

        private readonly int _sourceBytesPerSample;

        private string _fileName;

        private WaveStream _readerStream;

        #endregion

        #region Свойства

        public override WaveFormat WaveFormat
        {
            get { return _sampleChannel.WaveFormat; }
        }

        public override long Length
        {
            get { return _length; }
        }

        public override long Position
        {
            get { return SourceToDest(_readerStream.Position); }
            set
            {
                lock (_lockObject)
                {
                    _readerStream.Position = DestToSource(value);
                }
            }
        }

        public float Volume
        {
            get { return _sampleChannel.Volume; }
            set { _sampleChannel.Volume = value; }
        }

        #endregion

        #region Конструктор

        public AudioFileReader(string fileName)
        {
            _lockObject = new object();
            _fileName = fileName;
            CreateReaderStream(fileName);
            _sourceBytesPerSample = (_readerStream.WaveFormat.BitsPerSample / 8) * _readerStream.WaveFormat.Channels;
            _sampleChannel = new SampleChannel(_readerStream, false);
            _destBytesPerSample = 4 * _sampleChannel.WaveFormat.Channels;
            _length = SourceToDest(_readerStream.Length);
        }

        #endregion

        #region Методы

        private void CreateReaderStream(string fileName)
        {
            if (fileName.EndsWith(".wav", StringComparison.OrdinalIgnoreCase))
            {
                _readerStream = new WaveFileReader(fileName);
                if (_readerStream.WaveFormat.Encoding != WaveFormatEncoding.Pcm && _readerStream.WaveFormat.Encoding != WaveFormatEncoding.IeeeFloat)
                {
                    _readerStream = WaveFormatConversionStream.CreatePcmStream(_readerStream);
                    _readerStream = new BlockAlignReductionStream(_readerStream);
                }
            }
            else if (fileName.EndsWith(".mp3", StringComparison.OrdinalIgnoreCase))
            {
                _readerStream = new Mp3FileReader(fileName);
            }
            else if (fileName.EndsWith(".aiff"))
            {
                _readerStream = new AiffFileReader(fileName);
            }
            else
            {
                _readerStream = new MediaFoundationReader(fileName);
            }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            var waveBuffer = new WaveBuffer(buffer);
            int samplesRequired = count / 4;
            int samplesRead = Read(waveBuffer.FloatBuffer, offset / 4, samplesRequired);
            return samplesRead * 4;
        }

        public int Read(float[] buffer, int offset, int count)
        {
            lock (_lockObject)
            {
                return _sampleChannel.Read(buffer, offset, count);
            }
        }

        private long SourceToDest(long sourceBytes)
        {
            return _destBytesPerSample * (sourceBytes / _sourceBytesPerSample);
        }

        private long DestToSource(long destBytes)
        {
            return _sourceBytesPerSample * (destBytes / _destBytesPerSample);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _readerStream.Dispose();
                _readerStream = null;
            }
            base.Dispose(disposing);
        }

        #endregion
    }
}