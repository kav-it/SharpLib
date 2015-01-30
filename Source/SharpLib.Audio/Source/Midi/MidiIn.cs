using System;
using System.Runtime.InteropServices;

namespace SharpLib.Audio.Midi
{
    internal class MidiIn : IDisposable
    {
        #region Поля

        private readonly MidiInterop.MidiInCallback callback;

        private readonly IntPtr hMidiIn = IntPtr.Zero;

        private bool disposed;

        #endregion

        #region Свойства

        public static int NumberOfDevices
        {
            get { return MidiInterop.midiInGetNumDevs(); }
        }

        #endregion

        #region События

        public event EventHandler<MidiInMessageEventArgs> ErrorReceived;

        public event EventHandler<MidiInMessageEventArgs> MessageReceived;

        #endregion

        #region Конструктор

        public MidiIn(int deviceNo)
        {
            callback = Callback;
            MmException.Try(MidiInterop.midiInOpen(out hMidiIn, (IntPtr)deviceNo, callback, IntPtr.Zero, MidiInterop.CALLBACK_FUNCTION), "midiInOpen");
        }

        ~MidiIn()
        {
            System.Diagnostics.Debug.Assert(false, "MIDI In was not finalised");
            Dispose(false);
        }

        #endregion

        #region Методы

        public void Close()
        {
            Dispose();
        }

        public void Dispose()
        {
            GC.KeepAlive(callback);
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Start()
        {
            MmException.Try(MidiInterop.midiInStart(hMidiIn), "midiInStart");
        }

        public void Stop()
        {
            MmException.Try(MidiInterop.midiInStop(hMidiIn), "midiInStop");
        }

        public void Reset()
        {
            MmException.Try(MidiInterop.midiInReset(hMidiIn), "midiInReset");
        }

        private void Callback(IntPtr midiInHandle, MidiInterop.MidiInMessage message, IntPtr userData, IntPtr messageParameter1, IntPtr messageParameter2)
        {
            switch (message)
            {
                case MidiInterop.MidiInMessage.Open:

                    break;
                case MidiInterop.MidiInMessage.Data:

                    if (MessageReceived != null)
                    {
                        MessageReceived(this, new MidiInMessageEventArgs(messageParameter1.ToInt32(), messageParameter2.ToInt32()));
                    }
                    break;
                case MidiInterop.MidiInMessage.Error:

                    if (ErrorReceived != null)
                    {
                        ErrorReceived(this, new MidiInMessageEventArgs(messageParameter1.ToInt32(), messageParameter2.ToInt32()));
                    }
                    break;
                case MidiInterop.MidiInMessage.Close:

                    break;
                case MidiInterop.MidiInMessage.LongData:

                    break;
                case MidiInterop.MidiInMessage.LongError:

                    break;
                case MidiInterop.MidiInMessage.MoreData:

                    break;
            }
        }

        public static MidiInCapabilities DeviceInfo(int midiInDeviceNumber)
        {
            MidiInCapabilities caps = new MidiInCapabilities();
            int structSize = Marshal.SizeOf(caps);
            MmException.Try(MidiInterop.midiInGetDevCaps((IntPtr)midiInDeviceNumber, out caps, structSize), "midiInGetDevCaps");
            return caps;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                MidiInterop.midiInClose(hMidiIn);
            }
            disposed = true;
        }

        #endregion
    }
}