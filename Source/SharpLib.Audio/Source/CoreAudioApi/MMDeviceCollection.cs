using System.Collections.Generic;
using System.Runtime.InteropServices;

using SharpLib.Audio.CoreAudioApi.Interfaces;

namespace SharpLib.Audio.CoreAudioApi
{
    internal class MMDeviceCollection : IEnumerable<MMDevice>
    {
        #region Поля

        private readonly IMMDeviceCollection _MMDeviceCollection;

        #endregion

        #region Свойства

        public int Count
        {
            get
            {
                int result;
                Marshal.ThrowExceptionForHR(_MMDeviceCollection.GetCount(out result));
                return result;
            }
        }

        public MMDevice this[int index]
        {
            get
            {
                IMMDevice result;
                _MMDeviceCollection.Item(index, out result);
                return new MMDevice(result);
            }
        }

        #endregion

        #region Конструктор

        internal MMDeviceCollection(IMMDeviceCollection parent)
        {
            _MMDeviceCollection = parent;
        }

        #endregion

        #region Методы

        public IEnumerator<MMDevice> GetEnumerator()
        {
            for (int index = 0; index < Count; index++)
            {
                yield return this[index];
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}