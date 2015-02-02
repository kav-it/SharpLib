using System;
using System.Runtime.InteropServices;

using SharpLib.Audio.CoreAudioApi.Interfaces;

namespace SharpLib.Audio.Dmo
{
    [ComImport, Guid("f447b69e-1884-4a7e-8055-346f74d6edb3")]
    internal class ResamplerMediaComObject
    {
    }

    internal class DmoResampler : IDisposable
    {
        #region Поля

        private ResamplerMediaComObject mediaComObject;

        private MediaObject mediaObject;

        private IPropertyStore propertyStoreInterface;

        private IWMResamplerProps resamplerPropsInterface;

        #endregion

        #region Свойства

        public MediaObject MediaObject
        {
            get { return mediaObject; }
        }

        #endregion

        #region Конструктор

        public DmoResampler()
        {
            mediaComObject = new ResamplerMediaComObject();
            mediaObject = new MediaObject((IMediaObject)mediaComObject);
            propertyStoreInterface = (IPropertyStore)mediaComObject;
            resamplerPropsInterface = (IWMResamplerProps)mediaComObject;
        }

        #endregion

        #region Методы

        public void Dispose()
        {
            if (propertyStoreInterface != null)
            {
                Marshal.ReleaseComObject(propertyStoreInterface);
                propertyStoreInterface = null;
            }
            if (resamplerPropsInterface != null)
            {
                Marshal.ReleaseComObject(resamplerPropsInterface);
                resamplerPropsInterface = null;
            }
            if (mediaObject != null)
            {
                mediaObject.Dispose();
                mediaObject = null;
            }
            if (mediaComObject != null)
            {
                Marshal.ReleaseComObject(mediaComObject);
                mediaComObject = null;
            }
        }

        #endregion
    }
}