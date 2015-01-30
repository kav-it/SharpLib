using System.Runtime.InteropServices;

using SharpLib.Audio.CoreAudioApi.Interfaces;

namespace SharpLib.Audio.CoreAudioApi
{
    internal class PropertyStore
    {
        #region Поля

        private readonly IPropertyStore storeInterface;

        #endregion

        #region Свойства

        public int Count
        {
            get
            {
                int result;
                Marshal.ThrowExceptionForHR(storeInterface.GetCount(out result));
                return result;
            }
        }

        public PropertyStoreProperty this[int index]
        {
            get
            {
                PropVariant result;
                PropertyKey key = Get(index);
                Marshal.ThrowExceptionForHR(storeInterface.GetValue(ref key, out result));
                return new PropertyStoreProperty(key, result);
            }
        }

        public PropertyStoreProperty this[PropertyKey key]
        {
            get
            {
                PropVariant result;
                for (int i = 0; i < Count; i++)
                {
                    PropertyKey ikey = Get(i);
                    if ((ikey.formatId == key.formatId) && (ikey.propertyId == key.propertyId))
                    {
                        Marshal.ThrowExceptionForHR(storeInterface.GetValue(ref ikey, out result));
                        return new PropertyStoreProperty(ikey, result);
                    }
                }
                return null;
            }
        }

        #endregion

        #region Конструктор

        internal PropertyStore(IPropertyStore store)
        {
            storeInterface = store;
        }

        #endregion

        #region Методы

        public bool Contains(PropertyKey key)
        {
            for (int i = 0; i < Count; i++)
            {
                PropertyKey ikey = Get(i);
                if ((ikey.formatId == key.formatId) && (ikey.propertyId == key.propertyId))
                {
                    return true;
                }
            }
            return false;
        }

        public PropertyKey Get(int index)
        {
            PropertyKey key;
            Marshal.ThrowExceptionForHR(storeInterface.GetAt(index, out key));
            return key;
        }

        public PropVariant GetValue(int index)
        {
            PropVariant result;
            PropertyKey key = Get(index);
            Marshal.ThrowExceptionForHR(storeInterface.GetValue(ref key, out result));
            return result;
        }

        #endregion
    }
}