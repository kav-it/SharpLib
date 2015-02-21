using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Standard
{
    [StructLayout(LayoutKind.Explicit)]
    internal struct Win32Error
    {
        [FieldOffset(0)]
        private readonly int _value;

        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        public static readonly Win32Error ERROR_SUCCESS = new Win32Error(0);

        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        public static readonly Win32Error ERROR_INVALID_FUNCTION = new Win32Error(1);

        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        public static readonly Win32Error ERROR_FILE_NOT_FOUND = new Win32Error(2);

        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        public static readonly Win32Error ERROR_PATH_NOT_FOUND = new Win32Error(3);

        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        public static readonly Win32Error ERROR_TOO_MANY_OPEN_FILES = new Win32Error(4);

        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        public static readonly Win32Error ERROR_ACCESS_DENIED = new Win32Error(5);

        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        public static readonly Win32Error ERROR_INVALID_HANDLE = new Win32Error(6);

        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        public static readonly Win32Error ERROR_OUTOFMEMORY = new Win32Error(14);

        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        public static readonly Win32Error ERROR_NO_MORE_FILES = new Win32Error(18);

        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        public static readonly Win32Error ERROR_SHARING_VIOLATION = new Win32Error(32);

        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        public static readonly Win32Error ERROR_INVALID_PARAMETER = new Win32Error(87);

        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        public static readonly Win32Error ERROR_INSUFFICIENT_BUFFER = new Win32Error(122);

        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        public static readonly Win32Error ERROR_NESTING_NOT_ALLOWED = new Win32Error(215);

        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        public static readonly Win32Error ERROR_KEY_DELETED = new Win32Error(1018);

        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        public static readonly Win32Error ERROR_NOT_FOUND = new Win32Error(1168);

        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        public static readonly Win32Error ERROR_NO_MATCH = new Win32Error(1169);

        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        public static readonly Win32Error ERROR_BAD_DEVICE = new Win32Error(1200);

        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        public static readonly Win32Error ERROR_CANCELLED = new Win32Error(1223);

        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        public static readonly Win32Error ERROR_CLASS_ALREADY_EXISTS = new Win32Error(1410);

        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        public static readonly Win32Error ERROR_INVALID_DATATYPE = new Win32Error(1804);

        public Win32Error(int i)
        {
            _value = i;
        }

        public static explicit operator HRESULT(Win32Error error)
        {
            if (error._value <= 0)
            {
                return new HRESULT((uint)error._value);
            }
            return HRESULT.Make(true, Facility.Win32, error._value & 0x0000FFFF);
        }

        public HRESULT ToHresult()
        {
            return (HRESULT)this;
        }

        [SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands")]
        public static Win32Error GetLastError()
        {
            return new Win32Error(Marshal.GetLastWin32Error());
        }

        public override bool Equals(object obj)
        {
            try
            {
                return ((Win32Error)obj)._value == _value;
            }
            catch (InvalidCastException)
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return _value.GetHashCode();
        }

        public static bool operator ==(Win32Error errLeft, Win32Error errRight)
        {
            return errLeft._value == errRight._value;
        }

        public static bool operator !=(Win32Error errLeft, Win32Error errRight)
        {
            return !(errLeft == errRight);
        }
    }

    internal enum Facility
    {
        Null = 0,

        Rpc = 1,

        Dispatch = 2,

        Storage = 3,

        Itf = 4,

        Win32 = 7,

        Windows = 8,

        Control = 10,

        Ese = 0xE5E,

        WinCodec = 0x898,
    }

    [StructLayout(LayoutKind.Explicit)]
    internal struct HRESULT
    {
        [FieldOffset(0)]
        private readonly uint _value;

        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        public static readonly HRESULT S_OK = new HRESULT(0x00000000);

        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        public static readonly HRESULT S_FALSE = new HRESULT(0x00000001);

        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        public static readonly HRESULT E_PENDING = new HRESULT(0x8000000A);

        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        public static readonly HRESULT E_NOTIMPL = new HRESULT(0x80004001);

        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        public static readonly HRESULT E_NOINTERFACE = new HRESULT(0x80004002);

        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        public static readonly HRESULT E_POINTER = new HRESULT(0x80004003);

        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        public static readonly HRESULT E_ABORT = new HRESULT(0x80004004);

        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        public static readonly HRESULT E_FAIL = new HRESULT(0x80004005);

        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        public static readonly HRESULT E_UNEXPECTED = new HRESULT(0x8000FFFF);

        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        public static readonly HRESULT STG_E_INVALIDFUNCTION = new HRESULT(0x80030001);

        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        public static readonly HRESULT REGDB_E_CLASSNOTREG = new HRESULT(0x80040154);

        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        public static readonly HRESULT DESTS_E_NO_MATCHING_ASSOC_HANDLER = new HRESULT(0x80040F03);

        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        public static readonly HRESULT DESTS_E_NORECDOCS = new HRESULT(0x80040F04);

        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        public static readonly HRESULT DESTS_E_NOTALLCLEARED = new HRESULT(0x80040F05);

        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        public static readonly HRESULT E_ACCESSDENIED = new HRESULT(0x80070005);

        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        public static readonly HRESULT E_OUTOFMEMORY = new HRESULT(0x8007000E);

        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        public static readonly HRESULT E_INVALIDARG = new HRESULT(0x80070057);

        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        public static readonly HRESULT INTSAFE_E_ARITHMETIC_OVERFLOW = new HRESULT(0x80070216);

        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        public static readonly HRESULT COR_E_OBJECTDISPOSED = new HRESULT(0x80131622);

        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        public static readonly HRESULT WC_E_GREATERTHAN = new HRESULT(0xC00CEE23);

        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        public static readonly HRESULT WC_E_SYNTAX = new HRESULT(0xC00CEE2D);

        public HRESULT(uint i)
        {
            _value = i;
        }

        public static HRESULT Make(bool severe, Facility facility, int code)
        {
            Assert.Implies((int)facility != ((int)facility & 0x1FF), facility == Facility.Ese || facility == Facility.WinCodec);

            Assert.AreEqual(code, code & 0xFFFF);

            return new HRESULT((uint)((severe ? (1 << 31) : 0) | ((int)facility << 16) | code));
        }

        public Facility Facility
        {
            get { return GetFacility((int)_value); }
        }

        public static Facility GetFacility(int errorCode)
        {
            return (Facility)((errorCode >> 16) & 0x1fff);
        }

        public int Code
        {
            get { return GetCode((int)_value); }
        }

        public static int GetCode(int error)
        {
            return error & 0xFFFF;
        }

        #region Object class override members

        public override string ToString()
        {
            foreach (FieldInfo publicStaticField in typeof(HRESULT).GetFields(BindingFlags.Static | BindingFlags.Public))
            {
                if (publicStaticField.FieldType == typeof(HRESULT))
                {
                    var hr = (HRESULT)publicStaticField.GetValue(null);
                    if (hr == this)
                    {
                        return publicStaticField.Name;
                    }
                }
            }

            if (Facility == Facility.Win32)
            {
                foreach (FieldInfo publicStaticField in typeof(Win32Error).GetFields(BindingFlags.Static | BindingFlags.Public))
                {
                    if (publicStaticField.FieldType == typeof(Win32Error))
                    {
                        var error = (Win32Error)publicStaticField.GetValue(null);
                        if ((HRESULT)error == this)
                        {
                            return "HRESULT_FROM_WIN32(" + publicStaticField.Name + ")";
                        }
                    }
                }
            }

            return string.Format(CultureInfo.InvariantCulture, "0x{0:X8}", _value);
        }

        public override bool Equals(object obj)
        {
            try
            {
                return ((HRESULT)obj)._value == _value;
            }
            catch (InvalidCastException)
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return _value.GetHashCode();
        }

        #endregion

        public static bool operator ==(HRESULT hrLeft, HRESULT hrRight)
        {
            return hrLeft._value == hrRight._value;
        }

        public static bool operator !=(HRESULT hrLeft, HRESULT hrRight)
        {
            return !(hrLeft == hrRight);
        }

        public bool Succeeded
        {
            get { return (int)_value >= 0; }
        }

        public bool Failed
        {
            get { return (int)_value < 0; }
        }

        public void ThrowIfFailed()
        {
            ThrowIfFailed(null);
        }

        [
            SuppressMessage(
                "Microsoft.Usage",
                "CA2201:DoNotRaiseReservedExceptionTypes",
                Justification = "Only recreating Exceptions that were already raised."),
            SuppressMessage(
                "Microsoft.Security",
                "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands")
        ]
        public void ThrowIfFailed(string message)
        {
            if (Failed)
            {
                if (string.IsNullOrEmpty(message))
                {
                    message = ToString();
                }
#if DEBUG
                else
                {
                    message += " (" + ToString() + ")";
                }
#endif

                var e = Marshal.GetExceptionForHR((int)_value, new IntPtr(-1));
                Assert.IsNotNull(e);

                Assert.IsFalse(e is ArgumentNullException);

                if (e.GetType() == typeof(COMException))
                {
                    switch (Facility)
                    {
                        case Facility.Win32:
                            e = new Win32Exception(Code, message);
                            break;
                        default:
                            e = new COMException(message, (int)_value);
                            break;
                    }
                }
                else
                {
                    var cons = e.GetType().GetConstructor(new[] { typeof(string) });
                    if (null != cons)
                    {
                        e = cons.Invoke(new object[] { message }) as Exception;
                        Assert.IsNotNull(e);
                    }
                }
                if (e != null)
                {
                    throw e;
                }
            }
        }

        public static void ThrowLastError()
        {
            ((HRESULT)Win32Error.GetLastError()).ThrowIfFailed();

            Assert.Fail();
        }
    }
}