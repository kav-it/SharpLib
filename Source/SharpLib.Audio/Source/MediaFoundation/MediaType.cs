using System;
using System.Runtime.InteropServices;

using SharpLib.Audio.Utils;
using SharpLib.Audio.Wave;

namespace SharpLib.Audio.MediaFoundation
{
    internal class MediaType
    {
        #region Поля

        private readonly IMFMediaType mediaType;

        #endregion

        #region Свойства

        public int SampleRate
        {
            get { return GetUInt32(MediaFoundationAttributes.MF_MT_AUDIO_SAMPLES_PER_SECOND); }
            set { mediaType.SetUINT32(MediaFoundationAttributes.MF_MT_AUDIO_SAMPLES_PER_SECOND, value); }
        }

        public int ChannelCount
        {
            get { return GetUInt32(MediaFoundationAttributes.MF_MT_AUDIO_NUM_CHANNELS); }
            set { mediaType.SetUINT32(MediaFoundationAttributes.MF_MT_AUDIO_NUM_CHANNELS, value); }
        }

        public int BitsPerSample
        {
            get { return GetUInt32(MediaFoundationAttributes.MF_MT_AUDIO_BITS_PER_SAMPLE); }
            set { mediaType.SetUINT32(MediaFoundationAttributes.MF_MT_AUDIO_BITS_PER_SAMPLE, value); }
        }

        public int AverageBytesPerSecond
        {
            get { return GetUInt32(MediaFoundationAttributes.MF_MT_AUDIO_AVG_BYTES_PER_SECOND); }
        }

        public Guid SubType
        {
            get { return GetGuid(MediaFoundationAttributes.MF_MT_SUBTYPE); }
            set { mediaType.SetGUID(MediaFoundationAttributes.MF_MT_SUBTYPE, value); }
        }

        public Guid MajorType
        {
            get { return GetGuid(MediaFoundationAttributes.MF_MT_MAJOR_TYPE); }
            set { mediaType.SetGUID(MediaFoundationAttributes.MF_MT_MAJOR_TYPE, value); }
        }

        public IMFMediaType MediaFoundationObject
        {
            get { return mediaType; }
        }

        #endregion

        #region Конструктор

        public MediaType(IMFMediaType mediaType)
        {
            this.mediaType = mediaType;
        }

        public MediaType()
        {
            mediaType = MediaFoundationApi.CreateMediaType();
        }

        public MediaType(WaveFormat waveFormat)
        {
            mediaType = MediaFoundationApi.CreateMediaTypeFromWaveFormat(waveFormat);
        }

        #endregion

        #region Методы

        private int GetUInt32(Guid key)
        {
            int value;
            mediaType.GetUINT32(key, out value);
            return value;
        }

        private Guid GetGuid(Guid key)
        {
            Guid value;
            mediaType.GetGUID(key, out value);
            return value;
        }

        public int TryGetUInt32(Guid key, int defaultValue = -1)
        {
            int intValue = defaultValue;
            try
            {
                mediaType.GetUINT32(key, out intValue);
            }
            catch (COMException exception)
            {
                if (exception.GetHResult() == MediaFoundationErrors.MF_E_ATTRIBUTENOTFOUND)
                {
                }
                else if (exception.GetHResult() == MediaFoundationErrors.MF_E_INVALIDTYPE)
                {
                    throw new ArgumentException("Not a UINT32 parameter");
                }
                else
                {
                    throw;
                }
            }
            return intValue;
        }

        #endregion
    }
}