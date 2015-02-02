﻿using System.Runtime.InteropServices;

namespace SharpLib.Audio.Utils
{
    internal static class HResult
    {
        #region Константы

        public const int E_INVALIDARG = unchecked((int)0x80000003);

        private const int FACILITY_AAF = 18;

        private const int FACILITY_ACS = 20;

        private const int FACILITY_BACKGROUNDCOPY = 32;

        private const int FACILITY_CERT = 11;

        private const int FACILITY_COMPLUS = 17;

        private const int FACILITY_CONFIGURATION = 33;

        private const int FACILITY_CONTROL = 10;

        private const int FACILITY_DISPATCH = 2;

        private const int FACILITY_DPLAY = 21;

        private const int FACILITY_HTTP = 25;

        private const int FACILITY_INTERNET = 12;

        private const int FACILITY_ITF = 4;

        private const int FACILITY_MEDIASERVER = 13;

        private const int FACILITY_MSMQ = 14;

        private const int FACILITY_NULL = 0;

        private const int FACILITY_RPC = 1;

        private const int FACILITY_SCARD = 16;

        private const int FACILITY_SECURITY = 9;

        private const int FACILITY_SETUPAPI = 15;

        private const int FACILITY_SSPI = 9;

        private const int FACILITY_STORAGE = 3;

        private const int FACILITY_SXS = 23;

        private const int FACILITY_UMI = 22;

        private const int FACILITY_URT = 19;

        private const int FACILITY_WIN32 = 7;

        private const int FACILITY_WINDOWS = 8;

        private const int FACILITY_WINDOWS_CE = 24;

        public const int S_FALSE = 1;

        public const int S_OK = 0;

        #endregion

        #region Методы

        public static int MAKE_HRESULT(int sev, int fac, int code)
        {
            return (int)(((uint)sev) << 31 | ((uint)fac) << 16 | ((uint)code));
        }

        public static int GetHResult(this COMException exception)
        {
#if NETFX_CORE
            return exception.HResult;
#else
            return exception.ErrorCode;
#endif
        }

        #endregion
    }
}