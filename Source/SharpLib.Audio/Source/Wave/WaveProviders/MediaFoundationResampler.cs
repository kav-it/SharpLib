using System;
using System.Runtime.InteropServices;

using SharpLib.Audio.Dmo;
using SharpLib.Audio.MediaFoundation;

namespace SharpLib.Audio.Wave
{
    internal class MediaFoundationResampler : MediaFoundationTransform
    {
        #region Поля

        private static readonly Guid IMFTransformIid = new Guid("bf94c121-5b05-4e6f-8000-ba598961414d");

        private static readonly Guid ResamplerClsid = new Guid("f447b69e-1884-4a7e-8055-346f74d6edb3");

        private IMFActivate activate;

        private int resamplerQuality;

        #endregion

        #region Свойства

        public int ResamplerQuality
        {
            get { return resamplerQuality; }
            set
            {
                if (value < 1 || value > 60)
                {
                    throw new ArgumentOutOfRangeException("Resampler Quality must be between 1 and 60");
                }
                resamplerQuality = value;
            }
        }

        #endregion

        #region Конструктор

        public MediaFoundationResampler(IWaveProvider sourceProvider, WaveFormat outputFormat)
            : base(sourceProvider, outputFormat)
        {
            if (!IsPcmOrIeeeFloat(sourceProvider.WaveFormat))
            {
                throw new ArgumentException("Input must be PCM or IEEE float", "sourceProvider");
            }
            if (!IsPcmOrIeeeFloat(outputFormat))
            {
                throw new ArgumentException("Output must be PCM or IEEE float", "outputFormat");
            }
            MediaFoundationApi.Startup();
            ResamplerQuality = 60;

            var comObject = CreateResamplerComObject();
            FreeComObject(comObject);
        }

        public MediaFoundationResampler(IWaveProvider sourceProvider, int outputSampleRate)
            : this(sourceProvider, CreateOutputFormat(sourceProvider.WaveFormat, outputSampleRate))
        {
        }

        #endregion

        #region Методы

        private static bool IsPcmOrIeeeFloat(WaveFormat waveFormat)
        {
            var wfe = waveFormat as WaveFormatExtensible;
            return waveFormat.Encoding == WaveFormatEncoding.Pcm ||
                   waveFormat.Encoding == WaveFormatEncoding.IeeeFloat ||
                   (wfe != null && (wfe.SubFormat == AudioSubtypes.MFAudioFormat_PCM
                                    || wfe.SubFormat == AudioSubtypes.MFAudioFormat_Float));
        }

        private void FreeComObject(object comObject)
        {
            if (activate != null)
            {
                activate.ShutdownObject();
            }
            Marshal.ReleaseComObject(comObject);
        }

        private object CreateResamplerComObject()
        {
#if NETFX_CORE            
            return CreateResamplerComObjectUsingActivator();
#else
            return new ResamplerMediaComObject();
#endif
        }

        private object CreateResamplerComObjectUsingActivator()
        {
            var transformActivators = MediaFoundationApi.EnumerateTransforms(MediaFoundationTransformCategories.AudioEffect);
            foreach (var activator in transformActivators)
            {
                Guid clsid;
                activator.GetGUID(MediaFoundationAttributes.MFT_TRANSFORM_CLSID_Attribute, out clsid);
                if (clsid.Equals(ResamplerClsid))
                {
                    object comObject;
                    activator.ActivateObject(IMFTransformIid, out comObject);
                    activate = activator;
                    return comObject;
                }
            }
            return null;
        }

        protected override IMFTransform CreateTransform()
        {
            var comObject = CreateResamplerComObject();
            var resamplerTransform = (IMFTransform)comObject;

            var inputMediaFormat = MediaFoundationApi.CreateMediaTypeFromWaveFormat(sourceProvider.WaveFormat);
            resamplerTransform.SetInputType(0, inputMediaFormat, 0);
            Marshal.ReleaseComObject(inputMediaFormat);

            var outputMediaFormat = MediaFoundationApi.CreateMediaTypeFromWaveFormat(outputWaveFormat);
            resamplerTransform.SetOutputType(0, outputMediaFormat, 0);
            Marshal.ReleaseComObject(outputMediaFormat);

            var resamplerProps = (IWMResamplerProps)comObject;

            resamplerProps.SetHalfFilterLength(ResamplerQuality);

            return resamplerTransform;
        }

        private static WaveFormat CreateOutputFormat(WaveFormat inputFormat, int outputSampleRate)
        {
            WaveFormat outputFormat;
            if (inputFormat.Encoding == WaveFormatEncoding.Pcm)
            {
                outputFormat = new WaveFormat(outputSampleRate,
                    inputFormat.BitsPerSample,
                    inputFormat.Channels);
            }
            else if (inputFormat.Encoding == WaveFormatEncoding.IeeeFloat)
            {
                outputFormat = WaveFormat.CreateIeeeFloatWaveFormat(outputSampleRate,
                    inputFormat.Channels);
            }
            else
            {
                throw new ArgumentException("Can only resample PCM or IEEE float");
            }
            return outputFormat;
        }

        protected override void Dispose(bool disposing)
        {
            if (activate != null)
            {
                activate.ShutdownObject();
                activate = null;
            }

            base.Dispose(disposing);
        }

        #endregion
    }
}