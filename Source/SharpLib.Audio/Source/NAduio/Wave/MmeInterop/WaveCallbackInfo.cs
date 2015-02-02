using System;

namespace SharpLib.Audio.Wave
{
    internal class WaveCallbackInfo
    {
        #region Свойства

        public WaveCallbackStrategy Strategy { get; private set; }

        public IntPtr Handle { get; private set; }

        #endregion

        #region Конструктор

        private WaveCallbackInfo(WaveCallbackStrategy strategy, IntPtr handle)
        {
            Strategy = strategy;
            Handle = handle;
        }

        #endregion

        #region Методы

        public static WaveCallbackInfo FunctionCallback()
        {
            return new WaveCallbackInfo(WaveCallbackStrategy.FunctionCallback, IntPtr.Zero);
        }

        public static WaveCallbackInfo NewWindow()
        {
            return new WaveCallbackInfo(WaveCallbackStrategy.NewWindow, IntPtr.Zero);
        }

        public static WaveCallbackInfo ExistingWindow(IntPtr handle)
        {
            if (handle == IntPtr.Zero)
            {
                throw new ArgumentException("Handle cannot be zero");
            }
            return new WaveCallbackInfo(WaveCallbackStrategy.ExistingWindow, handle);
        }

        internal void Connect(WaveInterop.WaveCallback callback)
        {
            if (Strategy == WaveCallbackStrategy.NewWindow)
            {
            }
            else if (Strategy == WaveCallbackStrategy.ExistingWindow)
            {
            }
        }

        internal MmResult WaveOutOpen(out IntPtr waveOutHandle, int deviceNumber, WaveFormat waveFormat, WaveInterop.WaveCallback callback)
        {
            MmResult result;
            if (Strategy == WaveCallbackStrategy.FunctionCallback)
            {
                result = WaveInterop.waveOutOpen(out waveOutHandle, (IntPtr)deviceNumber, waveFormat, callback, IntPtr.Zero, WaveInterop.WaveInOutOpenFlags.CallbackFunction);
            }
            else
            {
                result = WaveInterop.waveOutOpenWindow(out waveOutHandle, (IntPtr)deviceNumber, waveFormat, Handle, IntPtr.Zero, WaveInterop.WaveInOutOpenFlags.CallbackWindow);
            }
            return result;
        }

        internal MmResult WaveInOpen(out IntPtr waveInHandle, int deviceNumber, WaveFormat waveFormat, WaveInterop.WaveCallback callback)
        {
            MmResult result;
            if (Strategy == WaveCallbackStrategy.FunctionCallback)
            {
                result = WaveInterop.waveInOpen(out waveInHandle, (IntPtr)deviceNumber, waveFormat, callback, IntPtr.Zero, WaveInterop.WaveInOutOpenFlags.CallbackFunction);
            }
            else
            {
                result = WaveInterop.waveInOpenWindow(out waveInHandle, (IntPtr)deviceNumber, waveFormat, Handle, IntPtr.Zero, WaveInterop.WaveInOutOpenFlags.CallbackWindow);
            }
            return result;
        }

        internal void Disconnect()
        {
        }

        #endregion
    }
}