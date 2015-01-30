using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace SharpLib.Audio.Dmo
{
    internal class DmoEnumerator
    {
        #region Методы

        public static IEnumerable<DmoDescriptor> GetAudioEffectNames()
        {
            return GetDmos(DmoGuids.DMOCATEGORY_AUDIO_EFFECT);
        }

        public static IEnumerable<DmoDescriptor> GetAudioEncoderNames()
        {
            return GetDmos(DmoGuids.DMOCATEGORY_AUDIO_ENCODER);
        }

        public static IEnumerable<DmoDescriptor> GetAudioDecoderNames()
        {
            return GetDmos(DmoGuids.DMOCATEGORY_AUDIO_DECODER);
        }

        private static IEnumerable<DmoDescriptor> GetDmos(Guid category)
        {
            IEnumDmo enumDmo;
            int hresult = DmoInterop.DMOEnum(ref category, DmoEnumFlags.None, 0, null, 0, null, out enumDmo);
            Marshal.ThrowExceptionForHR(hresult);
            Guid guid;
            int itemsFetched;
            IntPtr namePointer;
            do
            {
                enumDmo.Next(1, out guid, out namePointer, out itemsFetched);

                if (itemsFetched == 1)
                {
                    string name = Marshal.PtrToStringUni(namePointer);
                    Marshal.FreeCoTaskMem(namePointer);
                    yield return new DmoDescriptor(name, guid);
                }
            } while (itemsFetched > 0);
        }

        #endregion
    }
}