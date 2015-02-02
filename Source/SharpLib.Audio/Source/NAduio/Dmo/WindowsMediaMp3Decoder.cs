using System;
using System.Runtime.InteropServices;

using SharpLib.Audio.CoreAudioApi.Interfaces;

namespace SharpLib.Audio.Dmo
{
    [ComImport, Guid("bbeea841-0a63-4f52-a7ab-a9b3a84ed38a")]
    internal class WindowsMediaMp3DecoderComObject
    {
    }

    internal class WindowsMediaMp3Decoder : IDisposable
    {
        #region Поля

        private WindowsMediaMp3DecoderComObject mediaComObject;

        private MediaObject mediaObject;

        private IPropertyStore propertyStoreInterface;

        #endregion

        #region Свойства

        public MediaObject MediaObject
        {
            get { return mediaObject; }
        }

        #endregion

        #region Конструктор

        public WindowsMediaMp3Decoder()
        {
            mediaComObject = new WindowsMediaMp3DecoderComObject();
            mediaObject = new MediaObject((IMediaObject)mediaComObject);
            propertyStoreInterface = (IPropertyStore)mediaComObject;
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