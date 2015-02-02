﻿using SharpLib.Audio.Utils;

namespace SharpLib.Audio.CoreAudioApi.Interfaces
{
    internal enum AudioClientErrors
    {
        NotInitialized = unchecked((int)0x88890001),

        UnsupportedFormat = unchecked((int)0x88890008),

        DeviceInUse = unchecked((int)0x8889000A),
    }

    internal static class ErrorCodes
    {
        #region Константы

        private const int FACILITY_AUDCLNT = 0x889;

        private const int SEVERITY_ERROR = 1;

        #endregion

        #region Поля

        private static readonly int AUDCLNT_E_ALREADY_INITIALIZED = HResult.MAKE_HRESULT(SEVERITY_ERROR, FACILITY_AUDCLNT, 0x002);

        private static readonly int AUDCLNT_E_BUFDURATION_PERIOD_NOT_EQUAL = HResult.MAKE_HRESULT(SEVERITY_ERROR, FACILITY_AUDCLNT, 0x013);

        private static readonly int AUDCLNT_E_BUFFER_OPERATION_PENDING = HResult.MAKE_HRESULT(SEVERITY_ERROR, FACILITY_AUDCLNT, 0x00B);

        private static readonly int AUDCLNT_E_BUFFER_SIZE_ERROR = HResult.MAKE_HRESULT(SEVERITY_ERROR, FACILITY_AUDCLNT, 0x016);

        private static readonly int AUDCLNT_E_BUFFER_TOO_LARGE = HResult.MAKE_HRESULT(SEVERITY_ERROR, FACILITY_AUDCLNT, 0x006);

        private static readonly int AUDCLNT_E_CPUUSAGE_EXCEEDED = HResult.MAKE_HRESULT(SEVERITY_ERROR, FACILITY_AUDCLNT, 0x017);

        private static readonly int AUDCLNT_E_DEVICE_INVALIDATED = HResult.MAKE_HRESULT(SEVERITY_ERROR, FACILITY_AUDCLNT, 0x004);

        private static readonly int AUDCLNT_E_DEVICE_IN_USE = HResult.MAKE_HRESULT(SEVERITY_ERROR, FACILITY_AUDCLNT, 0x00A);

        private static readonly int AUDCLNT_E_ENDPOINT_CREATE_FAILED = HResult.MAKE_HRESULT(SEVERITY_ERROR, FACILITY_AUDCLNT, 0x00F);

        private static readonly int AUDCLNT_E_EVENTHANDLE_NOT_EXPECTED = HResult.MAKE_HRESULT(SEVERITY_ERROR, FACILITY_AUDCLNT, 0x011);

        private static readonly int AUDCLNT_E_EVENTHANDLE_NOT_SET = HResult.MAKE_HRESULT(SEVERITY_ERROR, FACILITY_AUDCLNT, 0x014);

        private static readonly int AUDCLNT_E_EXCLUSIVE_MODE_NOT_ALLOWED = HResult.MAKE_HRESULT(SEVERITY_ERROR, FACILITY_AUDCLNT, 0x00E);

        private static readonly int AUDCLNT_E_EXCLUSIVE_MODE_ONLY = HResult.MAKE_HRESULT(SEVERITY_ERROR, FACILITY_AUDCLNT, 0x0012);

        private static readonly int AUDCLNT_E_INCORRECT_BUFFER_SIZE = HResult.MAKE_HRESULT(SEVERITY_ERROR, FACILITY_AUDCLNT, 0x015);

        private static readonly int AUDCLNT_E_INVALID_SIZE = HResult.MAKE_HRESULT(SEVERITY_ERROR, FACILITY_AUDCLNT, 0x009);

        private static readonly int AUDCLNT_E_NOT_INITIALIZED = HResult.MAKE_HRESULT(SEVERITY_ERROR, FACILITY_AUDCLNT, 0x001);

        private static readonly int AUDCLNT_E_NOT_STOPPED = HResult.MAKE_HRESULT(SEVERITY_ERROR, FACILITY_AUDCLNT, 0x005);

        private static readonly int AUDCLNT_E_OUT_OF_ORDER = HResult.MAKE_HRESULT(SEVERITY_ERROR, FACILITY_AUDCLNT, 0x007);

        private static readonly int AUDCLNT_E_SERVICE_NOT_RUNNING = HResult.MAKE_HRESULT(SEVERITY_ERROR, FACILITY_AUDCLNT, 0x010);

        private static readonly int AUDCLNT_E_THREAD_NOT_REGISTERED = HResult.MAKE_HRESULT(SEVERITY_ERROR, FACILITY_AUDCLNT, 0x00C);

        private static readonly int AUDCLNT_E_UNSUPPORTED_FORMAT = HResult.MAKE_HRESULT(SEVERITY_ERROR, FACILITY_AUDCLNT, 0x008);

        private static readonly int AUDCLNT_E_WRONG_ENDPOINT_TYPE = HResult.MAKE_HRESULT(SEVERITY_ERROR, FACILITY_AUDCLNT, 0x003);

        #endregion
    }
}