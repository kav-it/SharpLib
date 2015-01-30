using System;
using System.Runtime.InteropServices;

using SharpLib.Audio.Wave;

namespace SharpLib.Audio.Dmo
{
    internal struct DmoMediaType
    {
        #region Поля

        private bool bFixedSizeSamples;

        private bool bTemporalCompression;

        private int cbFormat;

        private Guid formattype;

        private int lSampleSize;

        private Guid majortype;

        private IntPtr pUnk;

        private IntPtr pbFormat;

        private Guid subtype;

        #endregion

        #region Свойства

        public Guid MajorType
        {
            get { return majortype; }
        }

        public string MajorTypeName
        {
            get { return MediaTypes.GetMediaTypeName(majortype); }
        }

        public Guid SubType
        {
            get { return subtype; }
        }

        public string SubTypeName
        {
            get
            {
                if (majortype == MediaTypes.MEDIATYPE_Audio)
                {
                    return AudioMediaSubtypes.GetAudioSubtypeName(subtype);
                }
                return subtype.ToString();
            }
        }

        public bool FixedSizeSamples
        {
            get { return bFixedSizeSamples; }
        }

        public int SampleSize
        {
            get { return lSampleSize; }
        }

        public Guid FormatType
        {
            get { return formattype; }
        }

        public string FormatTypeName
        {
            get
            {
                if (formattype == DmoMediaTypeGuids.FORMAT_None)
                {
                    return "None";
                }
                if (formattype == Guid.Empty)
                {
                    return "Null";
                }
                if (formattype == DmoMediaTypeGuids.FORMAT_WaveFormatEx)
                {
                    return "WaveFormatEx";
                }
                return FormatType.ToString();
            }
        }

        #endregion

        #region Методы

        public WaveFormat GetWaveFormat()
        {
            if (formattype == DmoMediaTypeGuids.FORMAT_WaveFormatEx)
            {
                return WaveFormat.MarshalFromPtr(pbFormat);
            }
            throw new InvalidOperationException("Not a WaveFormat type");
        }

        public void SetWaveFormat(WaveFormat waveFormat)
        {
            majortype = MediaTypes.MEDIATYPE_Audio;

            WaveFormatExtensible wfe = waveFormat as WaveFormatExtensible;
            if (wfe != null)
            {
                subtype = wfe.SubFormat;
            }
            else
            {
                switch (waveFormat.Encoding)
                {
                    case WaveFormatEncoding.Pcm:
                        subtype = AudioMediaSubtypes.MEDIASUBTYPE_PCM;
                        break;
                    case WaveFormatEncoding.IeeeFloat:
                        subtype = AudioMediaSubtypes.MEDIASUBTYPE_IEEE_FLOAT;
                        break;
                    case WaveFormatEncoding.MpegLayer3:
                        subtype = AudioMediaSubtypes.WMMEDIASUBTYPE_MP3;
                        break;
                    default:
                        throw new ArgumentException(String.Format("Not a supported encoding {0}", waveFormat.Encoding));
                }
            }
            bFixedSizeSamples = (SubType == AudioMediaSubtypes.MEDIASUBTYPE_PCM || SubType == AudioMediaSubtypes.MEDIASUBTYPE_IEEE_FLOAT);
            formattype = DmoMediaTypeGuids.FORMAT_WaveFormatEx;
            if (cbFormat < Marshal.SizeOf(waveFormat))
            {
                throw new InvalidOperationException("Not enough memory assigned for a WaveFormat structure");
            }

            Marshal.StructureToPtr(waveFormat, pbFormat, false);
        }

        #endregion
    }
}