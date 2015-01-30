using System;
using System.Runtime.InteropServices;

namespace SharpLib.Audio.Midi
{
    internal class MidiOut : IDisposable
    {
        #region Поля

        private readonly MidiInterop.MidiOutCallback callback;

        private readonly IntPtr hMidiOut = IntPtr.Zero;

        private bool disposed;

        #endregion

        #region Свойства

        public static int NumberOfDevices
        {
            get { return MidiInterop.midiOutGetNumDevs(); }
        }

        public int Volume
        {
            get
            {
                int volume = 0;
                MmException.Try(MidiInterop.midiOutGetVolume(hMidiOut, ref volume), "midiOutGetVolume");
                return volume;
            }
            set { MmException.Try(MidiInterop.midiOutSetVolume(hMidiOut, value), "midiOutSetVolume"); }
        }

        #endregion

        #region Конструктор

        public MidiOut(int deviceNo)
        {
            callback = Callback;
            MmException.Try(MidiInterop.midiOutOpen(out hMidiOut, (IntPtr)deviceNo, callback, IntPtr.Zero, MidiInterop.CALLBACK_FUNCTION), "midiOutOpen");
        }

        ~MidiOut()
        {
            System.Diagnostics.Debug.Assert(false);
            Dispose(false);
        }

        #endregion

        #region Методы

        public static MidiOutCapabilities DeviceInfo(int midiOutDeviceNumber)
        {
            MidiOutCapabilities caps = new MidiOutCapabilities();
            int structSize = Marshal.SizeOf(caps);
            MmException.Try(MidiInterop.midiOutGetDevCaps((IntPtr)midiOutDeviceNumber, out caps, structSize), "midiOutGetDevCaps");
            return caps;
        }

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

        public void Reset()
        {
            MmException.Try(MidiInterop.midiOutReset(hMidiOut), "midiOutReset");
        }

        public void SendDriverMessage(int message, int param1, int param2)
        {
            MmException.Try(MidiInterop.midiOutMessage(hMidiOut, message, (IntPtr)param1, (IntPtr)param2), "midiOutMessage");
        }

        public void Send(int message)
        {
            MmException.Try(MidiInterop.midiOutShortMsg(hMidiOut, message), "midiOutShortMsg");
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                MidiInterop.midiOutClose(hMidiOut);
            }
            disposed = true;
        }

        private void Callback(IntPtr midiInHandle, MidiInterop.MidiOutMessage message, IntPtr userData, IntPtr messageParameter1, IntPtr messageParameter2)
        {
        }

        public void SendBuffer(byte[] byteBuffer)
        {
            var header = new MidiInterop.MIDIHDR();
            header.lpData = Marshal.AllocHGlobal(byteBuffer.Length);
            Marshal.Copy(byteBuffer, 0, header.lpData, byteBuffer.Length);

            header.dwBufferLength = byteBuffer.Length;
            header.dwBytesRecorded = byteBuffer.Length;
            int size = Marshal.SizeOf(header);
            MidiInterop.midiOutPrepareHeader(hMidiOut, ref header, size);
            var errcode = MidiInterop.midiOutLongMsg(hMidiOut, ref header, size);
            if (errcode != MmResult.NoError)
            {
                MidiInterop.midiOutUnprepareHeader(hMidiOut, ref header, size);
            }
            Marshal.FreeHGlobal(header.lpData);
        }

        #endregion
    }
}