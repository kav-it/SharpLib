// ****************************************************************************
//
// Имя файла    : 'Hardware.cs'
// Заголовок    : Модуль-помощник работы с оборудованием
// Автор        : Тихомиров В.С./Крыцкий А.В.
// Контакты     : kav.it@mail.ru
// Дата         : 17/05/2012
//
// ****************************************************************************

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Windows;

using Microsoft.Win32;

namespace SharpLib
{

    #region Класс OSInfo

    public class OperatingSystemInfo
    {
        #region Константы

        private const int PRODUCT_BUSINESS = 0x00000006;

        private const int PRODUCT_BUSINESS_N = 0x00000010;

        private const int PRODUCT_CLUSTER_SERVER = 0x00000012;

        private const int PRODUCT_CLUSTER_SERVER_V = 0x00000040;

        private const int PRODUCT_DATACENTER_SERVER = 0x00000008;

        private const int PRODUCT_DATACENTER_SERVER_CORE = 0x0000000C;

        private const int PRODUCT_DATACENTER_SERVER_CORE_V = 0x00000027;

        private const int PRODUCT_DATACENTER_SERVER_V = 0x00000025;

        private const int PRODUCT_EMBEDDED = 0x00000041;

        private const int PRODUCT_ENTERPRISE = 0x00000004;

        private const int PRODUCT_ENTERPRISE_E = 0x00000046;

        private const int PRODUCT_ENTERPRISE_N = 0x0000001B;

        private const int PRODUCT_ENTERPRISE_SERVER = 0x0000000A;

        private const int PRODUCT_ENTERPRISE_SERVER_CORE = 0x0000000E;

        private const int PRODUCT_ENTERPRISE_SERVER_CORE_V = 0x00000029;

        private const int PRODUCT_ENTERPRISE_SERVER_IA64 = 0x0000000F;

        private const int PRODUCT_ENTERPRISE_SERVER_V = 0x00000026;

        private const int PRODUCT_ESSENTIALBUSINESS_SERVER_ADDL = 0x0000003C;

        private const int PRODUCT_ESSENTIALBUSINESS_SERVER_ADDLSVC = 0x0000003E;

        private const int PRODUCT_ESSENTIALBUSINESS_SERVER_MGMT = 0x0000003B;

        private const int PRODUCT_ESSENTIALBUSINESS_SERVER_MGMTSVC = 0x0000003D;

        private const int PRODUCT_HOME_BASIC = 0x00000002;

        private const int PRODUCT_HOME_BASIC_E = 0x00000043;

        private const int PRODUCT_HOME_BASIC_N = 0x00000005;

        private const int PRODUCT_HOME_PREMIUM = 0x00000003;

        private const int PRODUCT_HOME_PREMIUM_E = 0x00000044;

        private const int PRODUCT_HOME_PREMIUM_N = 0x0000001A;

        private const int PRODUCT_HOME_PREMIUM_SERVER = 0x00000022;

        private const int PRODUCT_HOME_SERVER = 0x00000013;

        private const int PRODUCT_HYPERV = 0x0000002A;

        private const int PRODUCT_MEDIUMBUSINESS_SERVER_MANAGEMENT = 0x0000001E;

        private const int PRODUCT_MEDIUMBUSINESS_SERVER_MESSAGING = 0x00000020;

        private const int PRODUCT_MEDIUMBUSINESS_SERVER_SECURITY = 0x0000001F;

        private const int PRODUCT_PROFESSIONAL = 0x00000030;

        private const int PRODUCT_PROFESSIONAL_E = 0x00000045;

        private const int PRODUCT_PROFESSIONAL_N = 0x00000031;

        private const int PRODUCT_SB_SOLUTION_SERVER = 0x00000032;

        private const int PRODUCT_SB_SOLUTION_SERVER_EM = 0x00000036;

        private const int PRODUCT_SERVER_FOR_SB_SOLUTIONS = 0x00000033;

        private const int PRODUCT_SERVER_FOR_SB_SOLUTIONS_EM = 0x00000037;

        private const int PRODUCT_SERVER_FOR_SMALLBUSINESS = 0x00000018;

        private const int PRODUCT_SERVER_FOR_SMALLBUSINESS_V = 0x00000023;

        private const int PRODUCT_SERVER_FOUNDATION = 0x00000021;

        private const int PRODUCT_SMALLBUSINESS_SERVER = 0x00000009;

        private const int PRODUCT_SMALLBUSINESS_SERVER_PREMIUM = 0x00000019;

        private const int PRODUCT_SMALLBUSINESS_SERVER_PREMIUM_CORE = 0x0000003F;

        private const int PRODUCT_SOLUTION_EMBEDDEDSERVER = 0x00000038;

        private const int PRODUCT_SOLUTION_EMBEDDEDSERVER_CORE = 0x00000039;

        private const int PRODUCT_STANDARD_SERVER = 0x00000007;

        private const int PRODUCT_STANDARD_SERVER_CORE = 0x0000000D;

        private const int PRODUCT_STANDARD_SERVER_CORE_V = 0x00000028;

        private const int PRODUCT_STANDARD_SERVER_SOLUTIONS = 0x00000034;

        private const int PRODUCT_STANDARD_SERVER_SOLUTIONS_CORE = 0x00000035;

        private const int PRODUCT_STANDARD_SERVER_V = 0x00000024;

        private const int PRODUCT_STARTER = 0x0000000B;

        // private const int ????                                   = 0x0000003A;

        private const int PRODUCT_STARTER_E = 0x00000042;

        private const int PRODUCT_STARTER_N = 0x0000002F;

        private const int PRODUCT_STORAGE_ENTERPRISE_SERVER = 0x00000017;

        private const int PRODUCT_STORAGE_ENTERPRISE_SERVER_CORE = 0x0000002E;

        private const int PRODUCT_STORAGE_EXPRESS_SERVER = 0x00000014;

        private const int PRODUCT_STORAGE_EXPRESS_SERVER_CORE = 0x0000002B;

        private const int PRODUCT_STORAGE_STANDARD_SERVER = 0x00000015;

        private const int PRODUCT_STORAGE_STANDARD_SERVER_CORE = 0x0000002C;

        private const int PRODUCT_STORAGE_WORKGROUP_SERVER = 0x00000016;

        private const int PRODUCT_STORAGE_WORKGROUP_SERVER_CORE = 0x0000002D;

        private const int PRODUCT_ULTIMATE = 0x00000001;

        private const int PRODUCT_ULTIMATE_E = 0x00000047;

        private const int PRODUCT_ULTIMATE_N = 0x0000001C;

        private const int PRODUCT_UNDEFINED = 0x00000000;

        private const int PRODUCT_WEB_SERVER = 0x00000011;

        private const int PRODUCT_WEB_SERVER_CORE = 0x0000001D;

        // private const int PRODUCT_UNLICENSED                     = 0xABCDABCD;

        private const int VER_NT_DOMAIN_CONTROLLER = 2;

        private const int VER_NT_SERVER = 3;

        private const int VER_NT_WORKSTATION = 1;

        private const int VER_SUITE_BLADE = 1024;

        private const int VER_SUITE_DATACENTER = 128;

        private const int VER_SUITE_ENTERPRISE = 2;

        private const int VER_SUITE_PERSONAL = 512;

        private const int VER_SUITE_SINGLEUSERTS = 256;

        private const int VER_SUITE_SMALLBUSINESS = 1;

        private const int VER_SUITE_TERMINAL = 16;

        #endregion

        #region Поля

        private int _bits;

        private String _edition;

        private String _name;

        #endregion

        #region Свойства

        public int Bits
        {
            get
            {
                if (_bits == 0)
                {
                    if (IntPtr.Size == 8)
                        _bits = 64;
                    else
                    {
                        // Detect whether the current process is a 32-bit process 
                        // running on a 64-bit system.
                        bool flag;
                        bool result = ((DoesWin32MethodExist("kernel32.dll", "IsWow64Process") &&
                                        NativeMethods.IsWow64Process(NativeMethods.GetCurrentProcess(), out flag)) && flag);

                        if (result)
                            _bits = 64;
                        else
                            _bits = 32;
                    }
                }

                return _bits;
            }
        }

        public Boolean Is64Bit
        {
            get { return (Bits == 64); }
        }

        public String Name
        {
            get
            {
                if (_name == null)
                    _name = GetNameWindows();

                return _name;
            }
        }

        public String Edition
        {
            get
            {
                if (_edition == null)
                    _edition = GetEditionWindows();

                return _edition;
            }
        }

        public String ServicePack
        {
            get { return Environment.OSVersion.ServicePack; }
        }

        public String VersionString
        {
            get
            {
                String value = String.Format("{0} {1} {2} {3}-bits", Name, Edition, ServicePack, Bits);

                return value;
            }
        }

        public Version Version
        {
            get { return Environment.OSVersion.Version; }
        }

        #endregion

        #region Конструктор

        public OperatingSystemInfo()
        {
            _bits = 0;
            _edition = null;
            _name = null;
        }

        #endregion

        #region Методы

        private bool DoesWin32MethodExist(String moduleName, String methodName)
        {
            IntPtr moduleHandle = NativeMethods.GetModuleHandle(moduleName);
            if (moduleHandle == IntPtr.Zero)
                return false;
            return (NativeMethods.GetProcAddress(moduleHandle, methodName) != IntPtr.Zero);
        }

        private String GetNameWindows()
        {
            String name = "unknown";

            OperatingSystem osVersion = Environment.OSVersion;
            NativeMethods.OS_VERSION_INFO_EX osVersionInfo = new NativeMethods.OS_VERSION_INFO_EX();
            osVersionInfo.OsVersionInfoSize = Marshal.SizeOf(typeof(NativeMethods.OS_VERSION_INFO_EX));

            if (NativeMethods.GetVersionEx(ref osVersionInfo))
            {
                int majorVersion = osVersion.Version.Major;
                int minorVersion = osVersion.Version.Minor;

                switch (osVersion.Platform)
                {
                    case PlatformID.Win32S:
                        name = "Windows 3.1";
                        break;
                    case PlatformID.WinCE:
                        name = "Windows CE";
                        break;
                    case PlatformID.Win32Windows:
                        {
                            if (majorVersion == 4)
                            {
                                String csdVersion = osVersionInfo.CsdVersion;
                                switch (minorVersion)
                                {
                                    case 0:
                                        {
                                            if (csdVersion == "B" || csdVersion == "C")
                                                name = "Windows 95 OSR2";
                                            else
                                                name = "Windows 95";
                                        }
                                        break;
                                    case 10:
                                        {
                                            if (csdVersion == "A")
                                                name = "Windows 98 Second Edition";
                                            else
                                                name = "Windows 98";
                                        }
                                        break;
                                    case 90:
                                        name = "Windows Me";
                                        break;
                                }
                            }
                            break;
                        }
                    case PlatformID.Win32NT:
                        {
                            Byte productType = osVersionInfo.ProductTyp;

                            switch (majorVersion)
                            {
                                case 3:
                                    name = "Windows NT 3.51";
                                    break;
                                case 4:
                                    switch (productType)
                                    {
                                        case 1:
                                            name = "Windows NT 4.0";
                                            break;
                                        case 3:
                                            name = "Windows NT 4.0 Server";
                                            break;
                                    }
                                    break;
                                case 5:
                                    switch (minorVersion)
                                    {
                                        case 0:
                                            name = "Windows 2000";
                                            break;
                                        case 1:
                                            name = "Windows XP";
                                            break;
                                        case 2:
                                            name = "Windows Server 2003";
                                            break;
                                    }
                                    break;
                                case 6:
                                    switch (minorVersion)
                                    {
                                        case 0:
                                            switch (productType)
                                            {
                                                case 1:
                                                    name = "Windows Vista";
                                                    break;
                                                case 3:
                                                    name = "Windows Server 2008";
                                                    break;
                                            }
                                            break;

                                        case 1:
                                            switch (productType)
                                            {
                                                case 1:
                                                    name = "Windows 7";
                                                    break;
                                                case 3:
                                                    name = "Windows Server 2008 R2";
                                                    break;
                                            }
                                            break;
                                    }
                                    break;
                            }
                            break;
                        }
                } // end switch (osVersion.Platform)
            } // end if (GetVersion())

            return name;
        }

        private String GetEditionWindows()
        {
            String edition = String.Empty;

            OperatingSystem osVersion = Environment.OSVersion;
            NativeMethods.OS_VERSION_INFO_EX osVersionInfo = new NativeMethods.OS_VERSION_INFO_EX();
            osVersionInfo.OsVersionInfoSize = Marshal.SizeOf(typeof(NativeMethods.OS_VERSION_INFO_EX));

            if (NativeMethods.GetVersionEx(ref osVersionInfo))
            {
                int majorVersion = osVersion.Version.Major;
                int minorVersion = osVersion.Version.Minor;
                Byte productType = osVersionInfo.ProductTyp;
                short suiteMask = osVersionInfo.SuiteMask;

                if (majorVersion == 4)
                {
                    if (productType == VER_NT_WORKSTATION)
                    {
                        // Windows NT 4.0 Workstation
                        edition = "Workstation";
                    }
                    else if (productType == VER_NT_SERVER)
                    {
                        if ((suiteMask & VER_SUITE_ENTERPRISE) != 0)
                        {
                            // Windows NT 4.0 Server Enterprise
                            edition = "Enterprise Server";
                        }
                        else
                        {
                            // Windows NT 4.0 Server
                            edition = "Standard Server";
                        }
                    }
                }
                else if (majorVersion == 5)
                {
                    if (productType == VER_NT_WORKSTATION)
                    {
                        if ((suiteMask & VER_SUITE_PERSONAL) != 0)
                            edition = "Home";
                        else
                        {
                            // 86 == SM_TABLETPC
                            if (NativeMethods.GetSystemMetrics(86) == 0)
                                edition = "Professional";
                            else
                                edition = "Tablet Edition";
                        }
                    }
                    else if (productType == VER_NT_SERVER)
                    {
                        if (minorVersion == 0)
                        {
                            if ((suiteMask & VER_SUITE_DATACENTER) != 0)
                            {
                                // Windows 2000 Datacenter Server
                                edition = "Datacenter Server";
                            }
                            else if ((suiteMask & VER_SUITE_ENTERPRISE) != 0)
                            {
                                // Windows 2000 Advanced Server
                                edition = "Advanced Server";
                            }
                            else
                            {
                                // Windows 2000 Server
                                edition = "Server";
                            }
                        }
                        else
                        {
                            if ((suiteMask & VER_SUITE_DATACENTER) != 0)
                            {
                                // Windows Server 2003 Datacenter Edition
                                edition = "Datacenter";
                            }
                            else if ((suiteMask & VER_SUITE_ENTERPRISE) != 0)
                            {
                                // Windows Server 2003 Enterprise Edition
                                edition = "Enterprise";
                            }
                            else if ((suiteMask & VER_SUITE_BLADE) != 0)
                            {
                                // Windows Server 2003 Web Edition
                                edition = "Web Edition";
                            }
                            else
                            {
                                // Windows Server 2003 Standard Edition
                                edition = "Standard";
                            }
                        }
                    }
                }
                else if (majorVersion == 6)
                {
                    int ed;

                    if (NativeMethods.GetProductInfo(majorVersion, minorVersion, osVersionInfo.ServicePackMajor, osVersionInfo.ServicePackMinor, out ed))
                    {
                        switch (ed)
                        {
                            case PRODUCT_BUSINESS:
                                edition = "Business";
                                break;
                            case PRODUCT_BUSINESS_N:
                                edition = "Business N";
                                break;
                            case PRODUCT_CLUSTER_SERVER:
                                edition = "HPC Edition";
                                break;
                            case PRODUCT_CLUSTER_SERVER_V:
                                edition = "HPC Edition without Hyper-V";
                                break;
                            case PRODUCT_DATACENTER_SERVER:
                                edition = "Datacenter Server";
                                break;
                            case PRODUCT_DATACENTER_SERVER_CORE:
                                edition = "Datacenter Server (core installation)";
                                break;
                            case PRODUCT_DATACENTER_SERVER_V:
                                edition = "Datacenter Server without Hyper-V";
                                break;
                            case PRODUCT_DATACENTER_SERVER_CORE_V:
                                edition = "Datacenter Server without Hyper-V (core installation)";
                                break;
                            case PRODUCT_EMBEDDED:
                                edition = "Embedded";
                                break;
                            case PRODUCT_ENTERPRISE:
                                edition = "Enterprise";
                                break;
                            case PRODUCT_ENTERPRISE_N:
                                edition = "Enterprise N";
                                break;
                            case PRODUCT_ENTERPRISE_E:
                                edition = "Enterprise E";
                                break;
                            case PRODUCT_ENTERPRISE_SERVER:
                                edition = "Enterprise Server";
                                break;
                            case PRODUCT_ENTERPRISE_SERVER_CORE:
                                edition = "Enterprise Server (core installation)";
                                break;
                            case PRODUCT_ENTERPRISE_SERVER_CORE_V:
                                edition = "Enterprise Server without Hyper-V (core installation)";
                                break;
                            case PRODUCT_ENTERPRISE_SERVER_IA64:
                                edition = "Enterprise Server for Itanium-based Systems";
                                break;
                            case PRODUCT_ENTERPRISE_SERVER_V:
                                edition = "Enterprise Server without Hyper-V";
                                break;
                            case PRODUCT_ESSENTIALBUSINESS_SERVER_MGMT:
                                edition = "Essential Business Server MGMT";
                                break;
                            case PRODUCT_ESSENTIALBUSINESS_SERVER_ADDL:
                                edition = "Essential Business Server ADDL";
                                break;
                            case PRODUCT_ESSENTIALBUSINESS_SERVER_MGMTSVC:
                                edition = "Essential Business Server MGMTSVC";
                                break;
                            case PRODUCT_ESSENTIALBUSINESS_SERVER_ADDLSVC:
                                edition = "Essential Business Server ADDLSVC";
                                break;
                            case PRODUCT_HOME_BASIC:
                                edition = "Home Basic";
                                break;
                            case PRODUCT_HOME_BASIC_N:
                                edition = "Home Basic N";
                                break;
                            case PRODUCT_HOME_BASIC_E:
                                edition = "Home Basic E";
                                break;
                            case PRODUCT_HOME_PREMIUM:
                                edition = "Home Premium";
                                break;
                            case PRODUCT_HOME_PREMIUM_N:
                                edition = "Home Premium N";
                                break;
                            case PRODUCT_HOME_PREMIUM_E:
                                edition = "Home Premium E";
                                break;
                            case PRODUCT_HOME_PREMIUM_SERVER:
                                edition = "Home Premium Server";
                                break;
                            case PRODUCT_HYPERV:
                                edition = "Microsoft Hyper-V Server";
                                break;
                            case PRODUCT_MEDIUMBUSINESS_SERVER_MANAGEMENT:
                                edition = "Windows Essential Business Management Server";
                                break;
                            case PRODUCT_MEDIUMBUSINESS_SERVER_MESSAGING:
                                edition = "Windows Essential Business Messaging Server";
                                break;
                            case PRODUCT_MEDIUMBUSINESS_SERVER_SECURITY:
                                edition = "Windows Essential Business Security Server";
                                break;
                            case PRODUCT_PROFESSIONAL:
                                edition = "Professional";
                                break;
                            case PRODUCT_PROFESSIONAL_N:
                                edition = "Professional N";
                                break;
                            case PRODUCT_PROFESSIONAL_E:
                                edition = "Professional E";
                                break;
                            case PRODUCT_SB_SOLUTION_SERVER:
                                edition = "SB Solution Server";
                                break;
                            case PRODUCT_SB_SOLUTION_SERVER_EM:
                                edition = "SB Solution Server EM";
                                break;
                            case PRODUCT_SERVER_FOR_SB_SOLUTIONS:
                                edition = "Server for SB Solutions";
                                break;
                            case PRODUCT_SERVER_FOR_SB_SOLUTIONS_EM:
                                edition = "Server for SB Solutions EM";
                                break;
                            case PRODUCT_SERVER_FOR_SMALLBUSINESS:
                                edition = "Windows Essential Server Solutions";
                                break;
                            case PRODUCT_SERVER_FOR_SMALLBUSINESS_V:
                                edition = "Windows Essential Server Solutions without Hyper-V";
                                break;
                            case PRODUCT_SERVER_FOUNDATION:
                                edition = "Server Foundation";
                                break;
                            case PRODUCT_SMALLBUSINESS_SERVER:
                                edition = "Windows Small Business Server";
                                break;
                            case PRODUCT_SMALLBUSINESS_SERVER_PREMIUM:
                                edition = "Windows Small Business Server Premium";
                                break;
                            case PRODUCT_SMALLBUSINESS_SERVER_PREMIUM_CORE:
                                edition = "Windows Small Business Server Premium (core installation)";
                                break;
                            case PRODUCT_SOLUTION_EMBEDDEDSERVER:
                                edition = "Solution Embedded Server";
                                break;
                            case PRODUCT_SOLUTION_EMBEDDEDSERVER_CORE:
                                edition = "Solution Embedded Server (core installation)";
                                break;
                            case PRODUCT_STANDARD_SERVER:
                                edition = "Standard Server";
                                break;
                            case PRODUCT_STANDARD_SERVER_CORE:
                                edition = "Standard Server (core installation)";
                                break;
                            case PRODUCT_STANDARD_SERVER_SOLUTIONS:
                                edition = "Standard Server Solutions";
                                break;
                            case PRODUCT_STANDARD_SERVER_SOLUTIONS_CORE:
                                edition = "Standard Server Solutions (core installation)";
                                break;
                            case PRODUCT_STANDARD_SERVER_CORE_V:
                                edition = "Standard Server without Hyper-V (core installation)";
                                break;
                            case PRODUCT_STANDARD_SERVER_V:
                                edition = "Standard Server without Hyper-V";
                                break;
                            case PRODUCT_STARTER:
                                edition = "Starter";
                                break;
                            case PRODUCT_STARTER_N:
                                edition = "Starter N";
                                break;
                            case PRODUCT_STARTER_E:
                                edition = "Starter E";
                                break;
                            case PRODUCT_STORAGE_ENTERPRISE_SERVER:
                                edition = "Enterprise Storage Server";
                                break;
                            case PRODUCT_STORAGE_ENTERPRISE_SERVER_CORE:
                                edition = "Enterprise Storage Server (core installation)";
                                break;
                            case PRODUCT_STORAGE_EXPRESS_SERVER:
                                edition = "Express Storage Server";
                                break;
                            case PRODUCT_STORAGE_EXPRESS_SERVER_CORE:
                                edition = "Express Storage Server (core installation)";
                                break;
                            case PRODUCT_STORAGE_STANDARD_SERVER:
                                edition = "Standard Storage Server";
                                break;
                            case PRODUCT_STORAGE_STANDARD_SERVER_CORE:
                                edition = "Standard Storage Server (core installation)";
                                break;
                            case PRODUCT_STORAGE_WORKGROUP_SERVER:
                                edition = "Workgroup Storage Server";
                                break;
                            case PRODUCT_STORAGE_WORKGROUP_SERVER_CORE:
                                edition = "Workgroup Storage Server (core installation)";
                                break;
                            case PRODUCT_UNDEFINED:
                                edition = "Unknown product";
                                break;
                            case PRODUCT_ULTIMATE:
                                edition = "Ultimate";
                                break;
                            case PRODUCT_ULTIMATE_N:
                                edition = "Ultimate N";
                                break;
                            case PRODUCT_ULTIMATE_E:
                                edition = "Ultimate E";
                                break;
                            case PRODUCT_WEB_SERVER:
                                edition = "Web Server";
                                break;
                            case PRODUCT_WEB_SERVER_CORE:
                                edition = "Web Server (core installation)";
                                break;
                        } // end switch (ed)
                    } // end if (GetProductInfo())
                } // end else (else if (majorVersion == 6))
            } // end if (GetVersionEx(ref osVersionInfo))

            return edition;
        }

        #endregion
    }

    #endregion Класс OperatingSystemInfo

    #region Класс FrameworkVersionInfo

    public class FrameworkVersionInfo
    {
        #region Свойства

        public Boolean IsInstalled { get; set; }

        public String Title { get; set; }

        public String Version { get; set; }

        public String ServicePack { get; set; }

        public String VersionString
        {
            get { return String.Format("{0} {1}", Version, ServicePack); }
        }

        public String InstalledString
        {
            get
            {
                String text;

                if (IsInstalled)
                    text = String.Format("{0} - yes, ({1})", Title, VersionString);
                else
                    text = String.Format("{0} - no", Title);

                return text;
            }
        }

        #endregion

        #region Конструктор

        public FrameworkVersionInfo(String title)
        {
            Title = title;
        }

        #endregion
    }

    #endregion Класс FrameworkVersionInfo

    #region Класс FrameworkInfo

    public class FrameworkInfo
    {
        #region Константы

        private const String NET_INSTALL_REG_VALUE_NAME = "Install";

        private const String NET_INSTALL_ROOT_REG_KEY_NAME = "Software\\Microsoft\\.NETFramework";

        private const String NET_INSTALL_ROOT_REG_VALUE_NAME = "InstallRoot";

        private const String NET_SERVICE_PACK_REG_VALUE_NAME = "SP";

        private const String NET_V11_REG_KEY_NAME = "Software\\Microsoft\\NET Framework Setup\\NDP\\v1.1.4322";

        private const String NET_V20_REG_KEY_NAME = "Software\\Microsoft\\NET Framework Setup\\NDP\\v2.0.50727";

        private const String NET_V30_REG_KEY_NAME = "Software\\Microsoft\\NET Framework Setup\\NDP\\v3.0";

        private const String NET_V35_REG_KEY_NAME = "Software\\Microsoft\\NET Framework Setup\\NDP\\v3.5";

        private const String NET_V40_CLIENT_REG_KEY_NAME = "Software\\Microsoft\\NET Framework Setup\\NDP\\v4\\Client";

        private const String NET_V40_FULL_REG_KEY_NAME = "Software\\Microsoft\\NET Framework Setup\\NDP\\v4\\Full";

        private const String NET_VERSION_REG_VALUE_NAME = "Version";

        #endregion

        #region Поля

        private String _installPath;

        private FrameworkVersionInfo _v11;

        private FrameworkVersionInfo _v20;

        private FrameworkVersionInfo _v30;

        private FrameworkVersionInfo _v35;

        private FrameworkVersionInfo _v40Client;

        private FrameworkVersionInfo _v40Full;

        #endregion

        #region Свойства

        public String InstallPath
        {
            get { return _installPath; }
        }

        public FrameworkVersionInfo V11
        {
            get { return _v11; }
        }

        public FrameworkVersionInfo V20
        {
            get { return _v20; }
        }

        public FrameworkVersionInfo V30
        {
            get { return _v30; }
        }

        public FrameworkVersionInfo V35
        {
            get { return _v35; }
        }

        public FrameworkVersionInfo V40Client
        {
            get { return _v40Client; }
        }

        public FrameworkVersionInfo V40Full
        {
            get { return _v40Full; }
        }

        public String VersionString
        {
            get
            {
                String info = String.Format("{0}\r\n{1}\r\n{2}\r\n{3}\r\n{4}\r\n{5}",
                                            V11.InstalledString,
                                            V20.InstalledString,
                                            V30.InstalledString,
                                            V35.InstalledString,
                                            V40Client.InstalledString,
                                            V40Full.InstalledString);

                return info;
            }
        }

        #endregion

        #region Конструктор

        public FrameworkInfo()
        {
            InitInstallPath();

            _v11 = new FrameworkVersionInfo("1.1");
            _v20 = new FrameworkVersionInfo("2.0");
            _v30 = new FrameworkVersionInfo("3.0");
            _v35 = new FrameworkVersionInfo("3.5");
            _v40Client = new FrameworkVersionInfo("4.0 client");
            _v40Full = new FrameworkVersionInfo("4.0 full");

            InitFrameworkVersion(_v11, NET_V11_REG_KEY_NAME);
            InitFrameworkVersion(_v20, NET_V20_REG_KEY_NAME);
            InitFrameworkVersion(_v30, NET_V30_REG_KEY_NAME);
            InitFrameworkVersion(_v35, NET_V35_REG_KEY_NAME);
            InitFrameworkVersion(_v40Client, NET_V40_CLIENT_REG_KEY_NAME);
            InitFrameworkVersion(_v40Full, NET_V40_FULL_REG_KEY_NAME);
        }

        #endregion

        #region Методы

        private bool GetRegistryValue<T>(String key, String value, RegistryValueKind kind, out T data)
        {
            bool success = false;
            data = default(T);

            using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(key, RegistryKeyPermissionCheck.ReadSubTree))
            {
                if (registryKey != null)
                {
                    // If the key was opened, try to retrieve the value.
                    try
                    {
                        RegistryValueKind kindFound = registryKey.GetValueKind(value);
                        if (kindFound == kind)
                        {
                            object regValue = registryKey.GetValue(value, null);
                            if (regValue != null)
                            {
                                data = (T)Convert.ChangeType(regValue, typeof(T), CultureInfo.InvariantCulture);
                                success = true;
                            }
                        }
                    }
                    catch
                    {
                    }
                }
            }

            return success;
        }

        private void InitInstallPath()
        {
            String installPath;

            if (GetRegistryValue(NET_INSTALL_ROOT_REG_KEY_NAME, NET_INSTALL_ROOT_REG_VALUE_NAME,
                                 RegistryValueKind.String, out installPath) == false)
                installPath = "";

            _installPath = installPath;
        }

        private void InitFrameworkVersion(FrameworkVersionInfo framework, String key)
        {
            String installed;
            String version;

            framework.IsInstalled = false;
            if (GetRegistryValue(key, NET_INSTALL_REG_VALUE_NAME, RegistryValueKind.DWord, out installed))
            {
                if (GetRegistryValue(key, NET_VERSION_REG_VALUE_NAME, RegistryValueKind.String, out version))
                {
                    framework.IsInstalled = true;
                    framework.Version = version;

                    String sp;
                    if (GetRegistryValue(key, NET_SERVICE_PACK_REG_VALUE_NAME, RegistryValueKind.DWord, out sp))
                        framework.ServicePack = "Service Pack " + sp;
                }
            }
        }

        #endregion
    }

    #endregion FrameworkInfo

    #region ProcessorInfo

    public class ProcessorInfo
    {
        #region Свойства

        public String Title
        {
            get
            {
                RegistryKey rkey = Registry.LocalMachine;
                rkey = rkey.OpenSubKey("HARDWARE\\DESCRIPTION\\System\\CentralProcessor\\0");

                String text = (String)rkey.GetValue("ProcessorNameString");

                return text;
            }
        }

        #endregion
    }

    #endregion ProcessorInfo

    #region Класс MemoryInfo

    public class MemoryInfo
    {
        #region Поля

        private NativeMethods.MemoryStatusEx _memoryStatus;

        #endregion

        #region Свойства

        public UInt64 TotalPhySize
        {
            get { return _memoryStatus.TotalPhys; }
        }

        #endregion

        #region Конструктор

        public MemoryInfo()
        {
            _memoryStatus = new NativeMethods.MemoryStatusEx();
            NativeMethods.GlobalMemoryStatusEx(_memoryStatus);
        }

        #endregion
    }

    #endregion Класс MemoryInfo

    #region Класс MonitorInfo

    public class MonitorInfo
    {
        #region Поля

        private int _height;

        private int _width;

        #endregion

        #region Свойства

        public int Width
        {
            get { return _width; }
        }

        public int Height
        {
            get { return _height; }
        }

        public Double Scale
        {
            get
            {
                Double scale = GetScale();

                return scale;
            }
        }

        #endregion

        #region Конструктор

        public MonitorInfo()
        {
            IntPtr hwnd = NativeMethods.GetDesktopWindow();

            NativeMethods.RECT rect;
            NativeMethods.GetWindowRect(hwnd, out rect);

            _width = rect.Width;
            _height = rect.Height;
        }

        #endregion

        #region Методы

        private Double GetScale()
        {
            Double width = SystemParameters.FullPrimaryScreenWidth;

            Double scale = (Width / width);

            return scale;
        }

        #endregion
    }

    #endregion Класс MonitorInfo

    #region Класс SerialDeviceInfo

    public class SerialDeviceInfo
    {
        #region Свойства

        public String Name { get; set; }

        public String Desc { get; set; }

        #endregion

        #region Методы

        public override String ToString()
        {
            return Name + " ( " + Desc + " )";
        }

        #endregion
    }

    #endregion Класс SerialDeviceInfo

    #region Класс HardwareFolders

    public class HardwareFolders
    {
        #region Поля

        private String _desktop;

        private String _programFiles;

        private String _system32;

        #endregion

        #region Свойства

        public String ProgramFiles
        {
            get
            {
                if (_programFiles == null)
                    _programFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);

                return _programFiles;
            }
        }

        public String System32
        {
            get
            {
                if (_system32 == null)
                    _system32 = Environment.GetFolderPath(Environment.SpecialFolder.System);

                return _system32;
            }
        }

        public String Desktop
        {
            get
            {
                if (_desktop == null)
                    _desktop = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);

                return _desktop;
            }
        }

        #endregion

        #region Конструктор

        public HardwareFolders()
        {
            _programFiles = null;
            _system32 = null;
            _desktop = null;
        }

        #endregion
    }

    #endregion Класс HardwareFolders

    #region Класс Hardware

    public class Hardware
    {
        #region Свойства

        public static OperatingSystemInfo OsInfo { get; set; }

        public static FrameworkInfo DotNetInfo { get; set; }

        public static ProcessorInfo ProcInfo { get; set; }

        public static MemoryInfo MemoryInfo { get; set; }

        public static MonitorInfo MonitorInfo { get; set; }

        public static HardwareFolders Folders { get; set; }

        public static String MachineName
        {
            get { return Environment.MachineName; }
        }

        #endregion

        #region Конструктор

        static Hardware()
        {
            OsInfo = new OperatingSystemInfo();
            DotNetInfo = new FrameworkInfo();
            ProcInfo = new ProcessorInfo();
            MemoryInfo = new MemoryInfo();
            MonitorInfo = new MonitorInfo();
            Folders = new HardwareFolders();
        }

        #endregion

        #region Методы

        /// <summary>
        /// Перечисление всех последовательных портов в системе
        /// </summary>
        /// <returns></returns>
        public static List<SerialDeviceInfo> EnumSerial()
        {
            List<SerialDeviceInfo> list = new List<SerialDeviceInfo>();

            String[] kn;
            RegistryKey rKey = Registry.LocalMachine.OpenSubKey("HARDWARE");

            rKey.GetSubKeyNames();
            rKey = rKey.OpenSubKey("DEVICEMAP");
            rKey.GetSubKeyNames();
            rKey = rKey.OpenSubKey("SERIALCOMM");

            if (rKey != null)
            {
                kn = rKey.GetValueNames();

                foreach (String s in kn)
                {
                    SerialDeviceInfo device = new SerialDeviceInfo();

                    device.Name = s;
                    device.Desc = rKey.GetValue(s).ToString();

                    list.Add(device);
                }
            }

            return list;
        }

        public static IntPtr OpenDevice(String name)
        {
            unsafe
            {
                IntPtr handle = NativeMethods.CreateFile(
                                                         name,
                                                         NativeMethods.GENERIC_READ | NativeMethods.GENERIC_WRITE,
                                                         0,
                                                         (IntPtr)0,
                                                         NativeMethods.OPEN_EXISTING,
                                                         NativeMethods.FILE_ATTRIBUTE_NORMAL | NativeMethods.FILE_FLAG_OVERLAPPED,
                                                         null);

                NativeMethods.Check(handle);

                return handle;
            }
        }

        public static void CloseDevice(IntPtr handle)
        {
            NativeMethods.CloseHandle(handle);
        }

        #endregion
    }

    #endregion Класс Hardware
}